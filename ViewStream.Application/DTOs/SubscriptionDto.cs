using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a user's subscription plan.
    /// </summary>
    public class SubscriptionDto
    {
        /// <summary>Gets or sets the unique identifier of the subscription.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the subscription.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user who owns the subscription.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the tier or plan type (e.g. "Basic", "Standard", "Premium").</summary>
        public string PlanType { get; set; } = string.Empty;

        /// <summary>Gets or sets the current status of the subscription (e.g. "active", "cancelled", "past_due").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the date the current billing cycle started.</summary>
        public DateOnly StartDate { get; set; }

        /// <summary>Gets or sets the date the current billing cycle ends.</summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>Gets or sets a value indicating whether the subscription will automatically renew.</summary>
        public bool? AutoRenew { get; set; }

        /// <summary>Gets or sets the ID of the payment method associated with this subscription.</summary>
        public long? PaymentMethodId { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the subscription was initially created.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for creating a new subscription.
    /// </summary>
    public class CreateSubscriptionDto
    {
        /// <summary>Gets or sets the tier or plan type. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "PlanType is required.")]
        [MaxLength(50, ErrorMessage = "PlanType cannot exceed 50 characters.")]
        public string PlanType { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the payment method to use for billing.</summary>
        public long? PaymentMethodId { get; set; }

        /// <summary>Gets or sets a value indicating whether to auto-renew the subscription.</summary>
        public bool? AutoRenew { get; set; } = true;
    }

    /// <summary>
    /// Request body for updating an existing subscription.
    /// </summary>
    public class UpdateSubscriptionDto
    {
        /// <summary>Gets or sets the updated plan type. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "PlanType cannot exceed 50 characters.")]
        public string? PlanType { get; set; }

        /// <summary>Gets or sets the updated subscription status. Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string? Status { get; set; }

        /// <summary>Gets or sets the updated auto-renew preference.</summary>
        public bool? AutoRenew { get; set; }

        /// <summary>Gets or sets the updated default payment method ID.</summary>
        public long? PaymentMethodId { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for subscriptions shown in the admin dashboard.
    /// </summary>
    public class AdminSubscriptionListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the subscription.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the subscription.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user.</summary>
        public string? UserEmail { get; set; }

        /// <summary>Gets or sets the tier or plan type.</summary>
        public string? PlanType { get; set; }

        /// <summary>Gets or sets the current status of the subscription.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the date the current billing cycle started.</summary>
        public DateOnly StartDate { get; set; }

        /// <summary>Gets or sets the date the current billing cycle ends.</summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>Gets or sets a value indicating whether the subscription will auto-renew.</summary>
        public bool? AutoRenew { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the subscription was initially created.</summary>
        public DateTime? CreatedAt { get; set; }
    }
}
