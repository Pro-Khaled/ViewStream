using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("ProfileId", "EpisodeId", "DeviceId", Name = "UQ_OfflineDownloads_ProfileId_EpisodeId_DeviceId", IsUnique = true)]
public partial class OfflineDownload
{
    [Key]
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long EpisodeId { get; set; }

    public long DeviceId { get; set; }

    [StringLength(20)]
    public string? DownloadQuality { get; set; }

    public DateTime? DownloadedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    [StringLength(500)]
    public string? FilePath { get; set; }

    [ForeignKey("DeviceId")]
    [InverseProperty("OfflineDownloads")]
    public virtual Device Device { get; set; } = null!;

    [ForeignKey("EpisodeId")]
    [InverseProperty("OfflineDownloads")]
    public virtual Episode Episode { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [InverseProperty("OfflineDownloads")]
    public virtual Profile Profile { get; set; } = null!;
}
