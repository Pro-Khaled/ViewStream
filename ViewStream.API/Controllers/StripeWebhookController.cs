using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.API.Controllers
{
    /// <summary>
    /// Handles incoming Stripe webhook events.
    /// Verifies signatures and processes payment events.
    /// </summary>
    [ApiController]
    [Route("api/webhooks/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly string? _webhookSecret;

        public StripeWebhookController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<StripeWebhookController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webhookSecret = configuration["Stripe:WebhookSecret"];
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            string json;
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            _logger.LogInformation("Received Stripe webhook event.");

            Event stripeEvent;

            // Verify Stripe signature
            if (!string.IsNullOrEmpty(_webhookSecret))
            {
                var stripeSignature = Request.Headers["Stripe-Signature"];
                try
                {
                    stripeEvent = EventUtility.ConstructEvent(json, stripeSignature!, _webhookSecret);
                }
                catch (StripeException ex)
                {
                    _logger.LogWarning(ex, "Stripe webhook signature verification failed.");
                    return BadRequest("Invalid signature.");
                }
            }
            else
            {
                // Fallback: no webhook secret configured — parse event without verification (dev only)
                _logger.LogWarning("Stripe WebhookSecret not configured. Parsing event without signature verification.");
                try
                {
                    stripeEvent = EventUtility.ParseEvent(json);
                }
                catch (StripeException ex)
                {
                    _logger.LogError(ex, "Failed to parse Stripe event.");
                    return BadRequest("Invalid event payload.");
                }
            }

            _logger.LogInformation("Stripe event type: {EventType}, Id: {EventId}", stripeEvent.Type, stripeEvent.Id);

            try
            {
                switch (stripeEvent.Type)
                {
                    case EventTypes.InvoicePaymentSucceeded:
                        await HandlePaymentSucceeded(stripeEvent);
                        break;
                    case EventTypes.InvoicePaymentFailed:
                        await HandlePaymentFailed(stripeEvent);
                        break;
                    case EventTypes.CustomerSubscriptionDeleted:
                        await HandleSubscriptionDeleted(stripeEvent);
                        break;
                    default:
                        _logger.LogDebug("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook event {EventType}.", stripeEvent.Type);
                return StatusCode(500);
            }
        }

        private async Task HandlePaymentSucceeded(Event stripeEvent)
        {
            var invoice = stripeEvent.Data.Object as Stripe.Invoice;
            if (invoice == null)
            {
                _logger.LogWarning("Could not deserialize invoice from payment_succeeded event.");
                return;
            }

            var stripeCustomerId = invoice.CustomerId;
            if (string.IsNullOrEmpty(stripeCustomerId)) return;

            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(
                s => s.StripeCustomerId == stripeCustomerId && s.Status != "canceled");

            var sub = subscriptions.FirstOrDefault();
            if (sub == null) return;

            sub.Status = "active";
            sub.PaymentFailureCount = 0;
            _unitOfWork.Subscriptions.Update(sub);

            // Create invoice record
            var dbInvoice = new Domain.Entities.Invoice
            {
                UserId = sub.UserId,
                SubscriptionId = sub.Id,
                Amount = (invoice.AmountPaid ?? 0) / 100m,
                Currency = invoice.Currency?.ToUpperInvariant() ?? "USD",
                Status = "paid",
                InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
                PaidAt = DateTime.UtcNow,
                StripeInvoiceId = invoice.Id
            };

            await _unitOfWork.Invoices.AddAsync(dbInvoice);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Payment succeeded for Subscription {SubId}, Stripe Invoice {InvoiceId}",
                sub.Id, invoice.Id);
        }

        private async Task HandlePaymentFailed(Event stripeEvent)
        {
            var invoice = stripeEvent.Data.Object as Stripe.Invoice;
            if (invoice == null)
            {
                _logger.LogWarning("Could not deserialize invoice from payment_failed event.");
                return;
            }

            var stripeCustomerId = invoice.CustomerId;
            if (string.IsNullOrEmpty(stripeCustomerId)) return;

            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(
                s => s.StripeCustomerId == stripeCustomerId && s.Status != "canceled");

            var sub = subscriptions.FirstOrDefault();
            if (sub == null) return;

            sub.PaymentFailureCount++;
            sub.Status = sub.PaymentFailureCount >= 3 ? "canceled" : "past_due";
            _unitOfWork.Subscriptions.Update(sub);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning("Payment failed for Subscription {SubId} (attempt {Count}), Stripe Invoice {InvoiceId}",
                sub.Id, sub.PaymentFailureCount, invoice.Id);
        }

        private async Task HandleSubscriptionDeleted(Event stripeEvent)
        {
            var subscription = stripeEvent.Data.Object as Stripe.Subscription;
            if (subscription == null)
            {
                _logger.LogWarning("Could not deserialize subscription from subscription.deleted event.");
                return;
            }

            var stripeSubId = subscription.Id;
            if (string.IsNullOrEmpty(stripeSubId)) return;

            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(
                s => s.StripeSubscriptionId == stripeSubId);

            var sub = subscriptions.FirstOrDefault();
            if (sub == null) return;

            sub.Status = "canceled";
            _unitOfWork.Subscriptions.Update(sub);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Subscription {SubId} canceled via Stripe webhook (Stripe Sub: {StripeSubId}).",
                sub.Id, stripeSubId);
        }
    }
}
