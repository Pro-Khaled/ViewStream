using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("UserId", "PromoCodeId")]
[Table("UserPromoUsage")]
public partial class UserPromoUsage
{
    [Key]
    public long UserId { get; set; }

    [Key]
    public int PromoCodeId { get; set; }

    public DateTime? UsedAt { get; set; }

    [ForeignKey("PromoCodeId")]
    [InverseProperty("UserPromoUsages")]
    public virtual PromoCode PromoCode { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserPromoUsages")]
    public virtual User User { get; set; } = null!;
}
