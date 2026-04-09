using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("SeasonId", "EpisodeNumber", Name = "UQ_Episodes_SeasonId_EpisodeNumber", IsUnique = true)]
public partial class Episode
{
    [Key]
    public long Id { get; set; }

    public long SeasonId { get; set; }

    public short EpisodeNumber { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? RuntimeSeconds { get; set; }

    [StringLength(500)]
    public string VideoUrl { get; set; } = null!;

    [StringLength(500)]
    public string? ThumbnailUrl { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Episode")]
    public virtual ICollection<AudioTrack> AudioTracks { get; set; } = new List<AudioTrack>();

    [InverseProperty("Episode")]
    public virtual ICollection<ContentReport> ContentReports { get; set; } = new List<ContentReport>();

    [InverseProperty("Episode")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [InverseProperty("Episode")]
    public virtual ICollection<EpisodeComment> EpisodeComments { get; set; } = new List<EpisodeComment>();

    [InverseProperty("Episode")]
    public virtual ICollection<OfflineDownload> OfflineDownloads { get; set; } = new List<OfflineDownload>();

    [InverseProperty("Episode")]
    public virtual ICollection<PlaybackEvent> PlaybackEvents { get; set; } = new List<PlaybackEvent>();

    [ForeignKey("SeasonId")]
    [InverseProperty("Episodes")]
    public virtual Season Season { get; set; } = null!;

    [InverseProperty("Episode")]
    public virtual ICollection<Subtitle> Subtitles { get; set; } = new List<Subtitle>();

    [InverseProperty("Episode")]
    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();

    [InverseProperty("Episode")]
    public virtual ICollection<WatchParty> WatchParties { get; set; } = new List<WatchParty>();
}
