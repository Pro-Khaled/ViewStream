using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PromoCodeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public short? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidUntil { get; set; }
        public int? MaxUses { get; set; }
        public int? UsedCount { get; set; }
        public string? AppliesToPlan { get; set; }
        public bool IsValid { get; set; }
        public int RemainingUses { get; set; }
    }

    public class PromoCodeListItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public short? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateOnly ValidUntil { get; set; }
        public bool IsValid { get; set; }
        public int UsedCount { get; set; }
    }

    public class CreatePromoCodeDto
    {
        public string Code { get; set; } = string.Empty;
        public short? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidUntil { get; set; }
        public int? MaxUses { get; set; }
        public string? AppliesToPlan { get; set; }
    }

    public class UpdatePromoCodeDto
    {
        public short? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateOnly? ValidUntil { get; set; }
        public int? MaxUses { get; set; }
        public string? AppliesToPlan { get; set; }
    }

    public class ValidatePromoCodeDto
    {
        public string Code { get; set; } = string.Empty;
        public string? PlanType { get; set; }
    }

    public class PromoCodeValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public PromoCodeDto? PromoCode { get; set; }
        public decimal DiscountAmount { get; set; }
    }

}
