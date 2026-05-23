using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a promotional discount code.
    /// </summary>
    public class PromoCodeDto
    {
        /// <summary>Gets or sets the unique identifier of the promo code.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the actual string code applied by users.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the discount percentage off the total, if applicable.</summary>
        public short? DiscountPercent { get; set; }

        /// <summary>Gets or sets a fixed flat-rate discount amount, if applicable.</summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>Gets or sets the date the code becomes active.</summary>
        public DateOnly ValidFrom { get; set; }

        /// <summary>Gets or sets the date the code expires.</summary>
        public DateOnly? ValidUntil { get; set; }

        /// <summary>Gets or sets the maximum number of times this code can be used.</summary>
        public int? MaxUses { get; set; }

        /// <summary>Gets or sets the number of times this code has already been used.</summary>
        public int? UsedCount { get; set; }

        /// <summary>Gets or sets the specific subscription plan type this code applies to, if any.</summary>
        public string? AppliesToPlan { get; set; }

        /// <summary>Gets or sets a value indicating whether the code is currently valid and active.</summary>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets the remaining number of times this code can be used.</summary>
        public int RemainingUses { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a promo code.
    /// </summary>
    public class PromoCodeListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the promo code.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the string code.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the discount percentage.</summary>
        public short? DiscountPercent { get; set; }

        /// <summary>Gets or sets the fixed discount amount.</summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>Gets or sets the date the code expires.</summary>
        public DateOnly ValidUntil { get; set; }

        /// <summary>Gets or sets a value indicating whether the code is valid.</summary>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets the number of times this code has been used.</summary>
        public int UsedCount { get; set; }
    }

    /// <summary>
    /// Request body for creating a new promo code.
    /// </summary>
    public class CreatePromoCodeDto
    {
        /// <summary>Gets or sets the string code. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(50, ErrorMessage = "Code cannot exceed 50 characters.")]
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the discount percentage.</summary>
        [Range(1, 100, ErrorMessage = "DiscountPercent must be between 1 and 100.")]
        public short? DiscountPercent { get; set; }

        /// <summary>Gets or sets a fixed flat-rate discount amount.</summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "DiscountAmount must be greater than zero.")]
        public decimal? DiscountAmount { get; set; }

        /// <summary>Gets or sets the date the code becomes active.</summary>
        [Required(ErrorMessage = "ValidFrom is required.")]
        public DateOnly ValidFrom { get; set; }

        /// <summary>Gets or sets the date the code expires.</summary>
        public DateOnly? ValidUntil { get; set; }

        /// <summary>Gets or sets the maximum number of times this code can be used.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "MaxUses must be at least 1.")]
        public int? MaxUses { get; set; }

        /// <summary>Gets or sets the specific subscription plan type this applies to. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "AppliesToPlan cannot exceed 50 characters.")]
        public string? AppliesToPlan { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing promo code.
    /// </summary>
    public class UpdatePromoCodeDto
    {
        /// <summary>Gets or sets the updated discount percentage.</summary>
        [Range(1, 100, ErrorMessage = "DiscountPercent must be between 1 and 100.")]
        public short? DiscountPercent { get; set; }

        /// <summary>Gets or sets the updated fixed discount amount.</summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "DiscountAmount must be greater than zero.")]
        public decimal? DiscountAmount { get; set; }

        /// <summary>Gets or sets the updated expiration date.</summary>
        public DateOnly? ValidUntil { get; set; }

        /// <summary>Gets or sets the updated maximum number of uses.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "MaxUses must be at least 1.")]
        public int? MaxUses { get; set; }

        /// <summary>Gets or sets the updated subscription plan this applies to.</summary>
        [MaxLength(50, ErrorMessage = "AppliesToPlan cannot exceed 50 characters.")]
        public string? AppliesToPlan { get; set; }
    }

    /// <summary>
    /// Request body for validating a promo code during checkout.
    /// </summary>
    public class ValidatePromoCodeDto
    {
        /// <summary>Gets or sets the string code being applied.</summary>
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the target plan type being purchased.</summary>
        public string? PlanType { get; set; }
    }

    /// <summary>
    /// Response model returning the result of validating a promo code.
    /// </summary>
    public class PromoCodeValidationResultDto
    {
        /// <summary>Gets or sets a value indicating whether the code is valid for the request.</summary>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets a message explaining why the code is invalid, if applicable.</summary>
        public string? Message { get; set; }

        /// <summary>Gets or sets the valid promo code details.</summary>
        public PromoCodeDto? PromoCode { get; set; }

        /// <summary>Gets or sets the calculated discount amount that will be applied.</summary>
        public decimal DiscountAmount { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for promo codes shown in the admin dashboard.
    /// </summary>
    public class AdminPromoCodeListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the promo code.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the string code.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the discount percentage.</summary>
        public short? DiscountPercent { get; set; }

        /// <summary>Gets or sets the fixed discount amount.</summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>Gets or sets the date the code becomes active.</summary>
        public DateOnly ValidFrom { get; set; }

        /// <summary>Gets or sets the date the code expires.</summary>
        public DateOnly? ValidUntil { get; set; }

        /// <summary>Gets or sets a value indicating whether the code is valid.</summary>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets the number of times this code has been used.</summary>
        public int UsedCount { get; set; }

        /// <summary>Gets or sets the maximum number of uses allowed.</summary>
        public int? MaxUses { get; set; }

        /// <summary>Gets or sets the specific subscription plan this applies to.</summary>
        public string? AppliesToPlan { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the promo code was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the promo code was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
