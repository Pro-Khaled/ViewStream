using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("EpisodeId", "LanguageCode", Name = "UQ_Subtitles_EpisodeId_LanguageCode", IsUnique = true)]
public partial class Subtitle
{
    [Key]
    public long Id { get; set; }

    public long EpisodeId { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string LanguageCode { get; set; } = null!;

    [StringLength(500)]
    public string SubtitleUrl { get; set; } = null!;

    public bool? IsCc { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("Subtitles")]
    public virtual Episode Episode { get; set; } = null!;
}
