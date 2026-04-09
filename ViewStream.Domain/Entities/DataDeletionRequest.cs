using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class DataDeletionRequest
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateTime? RequestedAt { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateTime? CompletedAt { get; set; }

    [StringLength(100)]
    public string? ConfirmationCode { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("DataDeletionRequests")]
    public virtual User User { get; set; } = null!;
}
