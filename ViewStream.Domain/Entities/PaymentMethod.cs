using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class PaymentMethod
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(20)]
    public string Provider { get; set; } = null!;

    [StringLength(255)]
    public string ProviderToken { get; set; } = null!;

    [StringLength(4)]
    [Unicode(false)]
    public string? LastFour { get; set; }

    [StringLength(20)]
    public string? CardType { get; set; }

    public short? ExpiryMonth { get; set; }

    public short? ExpiryYear { get; set; }

    public bool? IsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PaymentMethods")]
    public virtual User User { get; set; } = null!;
}
