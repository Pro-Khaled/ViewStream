using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a promo code usage record by a user.
    /// </summary>
    public class UserPromoUsageDto
    {
        /// <summary>Gets or sets the ID of the user who used the promo code.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the promo code used.</summary>
        public int PromoCodeId { get; set; }

        /// <summary>Gets or sets the actual string code used.</summary>
        public string PromoCodeCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when the promo code was successfully applied.</summary>
        public DateTime? UsedAt { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for user promo code usage shown in the admin dashboard.
    /// </summary>
    public class AdminUserPromoUsageListItemDto
    {
        /// <summary>Gets or sets the ID of the user.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the ID of the promo code.</summary>
        public long PromoCodeId { get; set; }

        /// <summary>Gets or sets the actual string code used.</summary>
        public string? PromoCode { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the promo code was used.</summary>
        public DateTime? UsedAt { get; set; }
    }
}
