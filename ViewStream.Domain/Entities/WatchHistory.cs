using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Table("WatchHistory")]
[Index("ProfileId", Name = "IX_WatchHistory_ProfileId")]
public partial class WatchHistory
{
    [Key]
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long EpisodeId { get; set; }

    public DateTime? WatchedAt { get; set; }

    public int? ProgressSeconds { get; set; }

    public bool? Completed { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("WatchHistories")]
    public virtual Episode Episode { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [InverseProperty("WatchHistories")]
    public virtual Profile Profile { get; set; } = null!;
}
