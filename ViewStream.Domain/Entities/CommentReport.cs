using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class CommentReport
{
    [Key]
    public long Id { get; set; }

    public long CommentId { get; set; }

    public long ReportedByProfileId { get; set; }

    [StringLength(50)]
    public string Reason { get; set; } = null!;

    [StringLength(500)]
    public string? Details { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public long? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CommentId")]
    [InverseProperty("CommentReports")]
    public virtual EpisodeComment Comment { get; set; } = null!;

    [ForeignKey("ReportedByProfileId")]
    [InverseProperty("CommentReports")]
    public virtual Profile ReportedByProfile { get; set; } = null!;

    [ForeignKey("ReviewedByUserId")]
    [InverseProperty("CommentReports")]
    public virtual User? ReviewedByUser { get; set; }
}
