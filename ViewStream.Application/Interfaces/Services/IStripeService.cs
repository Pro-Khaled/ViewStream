namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Abstraction over Stripe payment operations.
    /// </summary>
    public interface IStripeService
    {
        /// <summary>Creates a Stripe Customer for the given user.</summary>
        Task<string> CreateCustomerAsync(long userId, string email, string? name = null);

        /// <summary>Creates a Stripe Subscription for the given customer.</summary>
        Task<string> CreateSubscriptionAsync(string customerId, string planType, string? promoCode = null);

        /// <summary>Updates (upgrades/downgrades) a Stripe Subscription with proration.</summary>
        Task UpdateSubscriptionAsync(string subscriptionId, string newPlanType, bool prorate = true);

        /// <summary>Cancels a Stripe Subscription at period end.</summary>
        Task CancelSubscriptionAsync(string subscriptionId);

        /// <summary>Retrieves invoice details from Stripe.</summary>
        Task<StripeInvoiceResult> GetInvoiceAsync(string invoiceId);

        /// <summary>Charges the customer for subscription renewal.</summary>
        Task<StripeChargeResult> ChargeForRenewalAsync(string customerId, decimal amount, string currency);
    }

    public class StripeInvoiceResult
    {
        public string InvoiceId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }

    public class StripeChargeResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
