using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class ErrorLog
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    [StringLength(50)]
    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StackTrace { get; set; }

    [StringLength(255)]
    public string? Endpoint { get; set; }

    public DateTime? OccurredAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ErrorLogs")]
    public virtual User? User { get; set; }
}
