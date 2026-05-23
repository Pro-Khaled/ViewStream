using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a payment method linked to a user.
    /// </summary>
    public class PaymentMethodDto
    {
        /// <summary>Gets or sets the unique identifier of the payment method.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user that owns this payment method.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the payment provider (e.g. "Stripe", "PayPal").</summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>Gets or sets the last four digits of the card or account.</summary>
        public string? LastFour { get; set; }

        /// <summary>Gets or sets the card brand or type (e.g. "Visa", "MasterCard").</summary>
        public string? CardType { get; set; }

        /// <summary>Gets or sets the expiry month (1-12) if applicable.</summary>
        public short? ExpiryMonth { get; set; }

        /// <summary>Gets or sets the expiry year (e.g. 2028) if applicable.</summary>
        public short? ExpiryYear { get; set; }

        /// <summary>Gets or sets a value indicating whether this is the default payment method for subscriptions.</summary>
        public bool? IsDefault { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this payment method was added.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for adding a new payment method.
    /// </summary>
    public class CreatePaymentMethodDto
    {
        /// <summary>Gets or sets the payment provider (e.g. "Stripe"). Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Provider is required.")]
        [MaxLength(50, ErrorMessage = "Provider cannot exceed 50 characters.")]
        public string Provider { get; set; } = string.Empty;

        /// <summary>Gets or sets the securely vaulted token returned by the payment gateway. Maximum 255 characters.</summary>
        [Required(ErrorMessage = "ProviderToken is required.")]
        [MaxLength(255, ErrorMessage = "ProviderToken cannot exceed 255 characters.")]
        public string ProviderToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the last four digits. Exactly 4 characters if provided.</summary>
        [StringLength(4, MinimumLength = 4, ErrorMessage = "LastFour must be exactly 4 characters.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "LastFour must contain only digits.")]
        public string? LastFour { get; set; }

        /// <summary>Gets or sets the card brand. Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "CardType cannot exceed 20 characters.")]
        public string? CardType { get; set; }

        /// <summary>Gets or sets the expiry month (1-12).</summary>
        [Range(1, 12, ErrorMessage = "ExpiryMonth must be between 1 and 12.")]
        public short? ExpiryMonth { get; set; }

        /// <summary>Gets or sets the expiry year.</summary>
        [Range(2024, 2100, ErrorMessage = "ExpiryYear is invalid.")]
        public short? ExpiryYear { get; set; }

        /// <summary>Gets or sets a value indicating whether this should become the default payment method.</summary>
        public bool? IsDefault { get; set; }
    }

    /// <summary>
    /// Request body for updating details of an existing payment method.
    /// Note: Sensitive details like tokens/cards cannot be updated directly; a new method should be added instead.
    /// </summary>
    public class UpdatePaymentMethodDto
    {
        /// <summary>Gets or sets the updated expiry month (1-12).</summary>
        [Range(1, 12, ErrorMessage = "ExpiryMonth must be between 1 and 12.")]
        public short? ExpiryMonth { get; set; }

        /// <summary>Gets or sets the updated expiry year.</summary>
        [Range(2024, 2100, ErrorMessage = "ExpiryYear is invalid.")]
        public short? ExpiryYear { get; set; }

        /// <summary>Gets or sets a value indicating whether this should be the default payment method.</summary>
        public bool? IsDefault { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for payment methods shown in the admin dashboard.
    /// </summary>
    public class AdminPaymentMethodListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the payment method.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user that owns this payment method.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the currency associated with the payment method, if applicable.</summary>
        public string? Currency { get; set; }

        /// <summary>Gets or sets a value indicating whether this is the default payment method.</summary>
        public bool? IsDefault { get; set; }

        /// <summary>Gets or sets the last four digits of the card or account.</summary>
        public string? LastFour { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this payment method was added.</summary>
        public DateTime? CreatedAt { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string? CardType { get; set; }
        public short? ExpiryMonth { get; set; }
        public short? ExpiryYear { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

