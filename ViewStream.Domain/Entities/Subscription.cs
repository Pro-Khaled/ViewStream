using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Subscription
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(20)]
    public string PlanType { get; set; } = null!;

    [StringLength(20)]
    public string? Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? AutoRenew { get; set; }

    public long? PaymentMethodId { get; set; }

    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Subscription")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("UserId")]
    [InverseProperty("Subscriptions")]
    public virtual User User { get; set; } = null!;
}
