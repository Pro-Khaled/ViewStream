using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("EpisodeId", "LanguageCode", "TrackType", Name = "UQ_AudioTracks_EpisodeId_LanguageCode_TrackType", IsUnique = true)]
public partial class AudioTrack
{
    [Key]
    public long Id { get; set; }

    public long EpisodeId { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string LanguageCode { get; set; } = null!;

    [StringLength(20)]
    public string? TrackType { get; set; }

    [StringLength(500)]
    public string AudioUrl { get; set; } = null!;

    public bool? IsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("AudioTracks")]
    public virtual Episode Episode { get; set; } = null!;
}
