using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class ContentReport
{
    [Key]
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long? ShowId { get; set; }

    public long? EpisodeId { get; set; }

    [StringLength(50)]
    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateTime? ReportedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("ContentReports")]
    public virtual Episode? Episode { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("ContentReports")]
    public virtual Profile Profile { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("ContentReports")]
    public virtual Show? Show { get; set; }
}
