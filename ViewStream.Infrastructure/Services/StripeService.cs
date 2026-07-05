using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Stripe service implementation using the Stripe.net SDK.
    /// Reads API keys and price IDs from configuration.
    /// 
    /// Required appsettings.json keys:
    ///   "Stripe": {
    ///     "SecretKey": "sk_test_...",
    ///     "WebhookSecret": "whsec_...",
    ///     "PriceIds": { "Basic": "price_...", "Standard": "price_...", "Premium": "price_..." }
    ///   }
    /// </summary>
    public class StripeService : IStripeService
    {
        private readonly ILogger<StripeService> _logger;
        private readonly Dictionary<string, string> _planToPriceId;

        public StripeService(IConfiguration configuration, ILogger<StripeService> logger)
        {
            _logger = logger;

            var secretKey = configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey))
            {
                StripeConfiguration.ApiKey = secretKey;
                _logger.LogInformation("Stripe service initialized with API key.");
            }
            else
            {
                _logger.LogWarning("Stripe SecretKey not configured. Stripe operations will fail.");
            }

            // Load plan-to-price mappings from configuration, with sensible defaults
            _planToPriceId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Basic"]    = configuration["Stripe:PriceIds:Basic"]    ?? "price_basic_monthly",
                ["Standard"] = configuration["Stripe:PriceIds:Standard"] ?? "price_standard_monthly",
                ["Premium"]  = configuration["Stripe:PriceIds:Premium"]  ?? "price_premium_monthly"
            };
        }

        public async Task<string> CreateCustomerAsync(long userId, string email, string? name = null)
        {
            _logger.LogInformation("Creating Stripe customer for UserId: {UserId}, Email: {Email}", userId, email);

            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Metadata = new Dictionary<string, string> { { "userId", userId.ToString() } }
            };

            var service = new CustomerService();
            var customer = await service.CreateAsync(options);

            _logger.LogInformation("Stripe customer created: {CustomerId}", customer.Id);
            return customer.Id;
        }

        public async Task<string> CreateSubscriptionAsync(string customerId, string planType, string? promoCode = null)
        {
            _logger.LogInformation("Creating Stripe subscription: Customer={CustomerId}, Plan={PlanType}, Promo={PromoCode}",
                customerId, planType, promoCode);

            var priceId = _planToPriceId.GetValueOrDefault(planType, _planToPriceId["Basic"]);

            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new() { Price = priceId }
                }
            };

            if (!string.IsNullOrEmpty(promoCode))
                options.PromotionCode = promoCode;

            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(options);

            _logger.LogInformation("Stripe subscription created: {SubscriptionId}", subscription.Id);
            return subscription.Id;
        }

        public async Task UpdateSubscriptionAsync(string subscriptionId, string newPlanType, bool prorate = true)
        {
            _logger.LogInformation("Updating Stripe subscription {SubscriptionId} to plan {PlanType}, Prorate={Prorate}",
                subscriptionId, newPlanType, prorate);

            var service = new SubscriptionService();
            var subscription = await service.GetAsync(subscriptionId);

            var newPriceId = _planToPriceId.GetValueOrDefault(newPlanType, _planToPriceId["Basic"]);

            var options = new SubscriptionUpdateOptions
            {
                Items = new List<SubscriptionItemOptions>
                {
                    new()
                    {
                        Id = subscription.Items.Data[0].Id,
                        Price = newPriceId
                    }
                },
                ProrationBehavior = prorate ? "create_prorations" : "none"
            };

            await service.UpdateAsync(subscriptionId, options);

            _logger.LogInformation("Stripe subscription updated: {SubscriptionId} -> {PlanType}", subscriptionId, newPlanType);
        }

        public async Task CancelSubscriptionAsync(string subscriptionId)
        {
            _logger.LogInformation("Canceling Stripe subscription: {SubscriptionId}", subscriptionId);

            var service = new SubscriptionService();
            await service.CancelAsync(subscriptionId, new SubscriptionCancelOptions
            {
                InvoiceNow = false,
                Prorate = false
            });

            _logger.LogInformation("Stripe subscription canceled: {SubscriptionId}", subscriptionId);
        }

        public async Task<StripeInvoiceResult> GetInvoiceAsync(string invoiceId)
        {
            _logger.LogInformation("Getting Stripe invoice: {InvoiceId}", invoiceId);

            var service = new InvoiceService();
            var invoice = await service.GetAsync(invoiceId);

            return new StripeInvoiceResult
            {
                InvoiceId = invoice.Id,
                Amount = (invoice.AmountPaid ?? 0) / 100m,
                Currency = invoice.Currency?.ToUpperInvariant() ?? "USD",
                Status = invoice.Status ?? "unknown",
                PaidAt = invoice.StatusTransitions?.PaidAt
            };
        }

        public async Task<StripeChargeResult> ChargeForRenewalAsync(string customerId, decimal amount, string currency)
        {
            _logger.LogInformation("Charging {Amount} {Currency} for customer {CustomerId}", amount, currency, customerId);

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency.ToLowerInvariant(),
                Customer = customerId,
                Confirm = true,
                OffSession = true,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                }
            };

            var service = new PaymentIntentService();
            try
            {
                var intent = await service.CreateAsync(options);
                _logger.LogInformation("Payment succeeded for customer {CustomerId}: {TransactionId}", customerId, intent.Id);
                return new StripeChargeResult { Success = intent.Status == "succeeded", TransactionId = intent.Id };
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Payment failed for customer {CustomerId}: {Message}", customerId, ex.Message);
                return new StripeChargeResult { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}
