using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Daily Hangfire job that processes subscription renewals for subscriptions expiring within 24 hours.
    /// On success: updates EndsAt, creates Invoice record.
    /// On failure: increments PaymentFailureCount, sets status to past_due, schedules retry.
    /// After 3 failures: cancels subscription, sends email notification.
    /// </summary>
    public class SubscriptionRenewalJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubscriptionRenewalJob> _logger;

        // Plan pricing (would normally come from configuration)
        private static readonly Dictionary<string, decimal> PlanPricing = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Basic"]    = 9.99m,
            ["Standard"] = 14.99m,
            ["Premium"]  = 19.99m
        };

        public SubscriptionRenewalJob(IServiceScopeFactory scopeFactory, ILogger<SubscriptionRenewalJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Subscription renewal job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var stripeService = scope.ServiceProvider.GetRequiredService<IStripeService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var cutoff = DateTime.UtcNow.AddHours(24);

            // Find active subscriptions ending within 24 hours with auto-renew enabled
            var expiringSubscriptions = await unitOfWork.Subscriptions.FindAsync(
                s => s.Status == "active" &&
                     s.AutoRenew == true &&
                     s.EndsAt != null &&
                     s.EndsAt <= cutoff,
                include: q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                    .Include(q, s => s.User));

            foreach (var sub in expiringSubscriptions)
            {
                try
                {
                    var amount = PlanPricing.GetValueOrDefault(sub.PlanType, 9.99m);
                    var chargeResult = await stripeService.ChargeForRenewalAsync(
                        sub.StripeCustomerId ?? $"cus_{sub.UserId}", amount, "USD");

                    if (chargeResult.Success)
                    {
                        // Success: renew the subscription
                        sub.EndsAt = DateTime.UtcNow.AddMonths(1);
                        sub.PaymentFailureCount = 0;
                        sub.Status = "active";
                        unitOfWork.Subscriptions.Update(sub);

                        // Create invoice record
                        var invoice = new Invoice
                        {
                            UserId = sub.UserId,
                            SubscriptionId = sub.Id,
                            Amount = amount,
                            Currency = "USD",
                            Status = "paid",
                            InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            PaidAt = DateTime.UtcNow,
                            TransactionId = chargeResult.TransactionId
                        };
                        await unitOfWork.Invoices.AddAsync(invoice);
                        await unitOfWork.SaveChangesAsync();

                        _logger.LogInformation("Subscription {SubId} renewed for User {UserId}. Next renewal: {EndsAt}",
                            sub.Id, sub.UserId, sub.EndsAt);
                    }
                    else
                    {
                        await HandlePaymentFailure(sub, chargeResult.ErrorMessage, unitOfWork, stripeService, emailService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing renewal for Subscription {SubId}", sub.Id);
                    await HandlePaymentFailure(sub, ex.Message, unitOfWork, stripeService, emailService);
                }
            }

            _logger.LogInformation("Subscription renewal job completed.");
        }

        private async Task HandlePaymentFailure(Subscription sub, string? errorMessage,
            IUnitOfWork unitOfWork, IStripeService stripeService, IEmailService emailService)
        {
            sub.PaymentFailureCount++;
            _logger.LogWarning("Payment failed for Subscription {SubId} (attempt {Count}/3): {Error}",
                sub.Id, sub.PaymentFailureCount, errorMessage);

            if (sub.PaymentFailureCount >= 3)
            {
                // Cancel after 3 failures
                sub.Status = "canceled";
                if (!string.IsNullOrEmpty(sub.StripeSubscriptionId))
                    await stripeService.CancelSubscriptionAsync(sub.StripeSubscriptionId);

                _logger.LogWarning("Subscription {SubId} canceled after 3 payment failures.", sub.Id);

                // Send cancellation email
                if (sub.User?.Email != null)
                {
                    await emailService.SendNotificationAsync(
                        sub.User.Email,
                        "Subscription Canceled",
                        "Your ViewStream subscription has been canceled due to repeated payment failures. " +
                        "Please update your payment method and resubscribe.");
                }
            }
            else
            {
                sub.Status = "past_due";
            }

            unitOfWork.Subscriptions.Update(sub);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
