using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Invoice
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? SubscriptionId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(3)]
    [Unicode(false)]
    public string? Currency { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public DateTime? PaidAt { get; set; }

    [StringLength(500)]
    public string? InvoicePdfUrl { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    [ForeignKey("SubscriptionId")]
    [InverseProperty("Invoices")]
    public virtual Subscription? Subscription { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Invoices")]
    public virtual User? User { get; set; }
}
