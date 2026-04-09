using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("Code", Name = "UQ__PromoCod__A25C5AA7AA56D5B1", IsUnique = true)]
public partial class PromoCode
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    public short? DiscountPercent { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DiscountAmount { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly? ValidUntil { get; set; }

    public int? MaxUses { get; set; }

    public int? UsedCount { get; set; }

    public string? AppliesToPlan { get; set; }

    [InverseProperty("PromoCode")]
    public virtual ICollection<UserPromoUsage> UserPromoUsages { get; set; } = new List<UserPromoUsage>();
}
