using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class PlaybackEvent
{
    [Key]
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long EpisodeId { get; set; }

    [StringLength(30)]
    public string EventType { get; set; } = null!;

    public int? PositionSeconds { get; set; }

    [StringLength(20)]
    public string? Quality { get; set; }

    public int? BitrateKbps { get; set; }

    public int? BufferingCount { get; set; }

    [StringLength(50)]
    public string? DeviceType { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("PlaybackEvents")]
    public virtual Episode Episode { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [InverseProperty("PlaybackEvents")]
    public virtual Profile Profile { get; set; } = null!;
}
