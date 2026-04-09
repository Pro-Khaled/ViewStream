using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("ChangedAt", Name = "IX_AuditLogs_ChangedAt")]
[Index("ChangedByUserId", Name = "IX_AuditLogs_ChangedByUserId")]
[Index("TableName", "RecordId", Name = "IX_AuditLogs_TableName_RecordId")]
public partial class AuditLog
{
    [Key]
    public long Id { get; set; }

    [StringLength(100)]
    public string TableName { get; set; } = null!;

    public long RecordId { get; set; }

    [StringLength(20)]
    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public long? ChangedByUserId { get; set; }

    public DateTime? ChangedAt { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [ForeignKey("ChangedByUserId")]
    [InverseProperty("AuditLogs")]
    public virtual User? ChangedByUser { get; set; }
}
