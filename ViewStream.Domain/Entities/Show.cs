using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Show
{
    [Key]
    public long Id { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public short? ReleaseYear { get; set; }

    [StringLength(10)]
    public string? MaturityRating { get; set; }

    public short? RuntimeMinutes { get; set; }

    [StringLength(500)]
    public string? PosterUrl { get; set; }

    [StringLength(500)]
    public string? BackdropUrl { get; set; }

    [StringLength(500)]
    public string? TrailerUrl { get; set; }

    [Column(TypeName = "decimal(3, 1)")]
    public decimal? ImdbRating { get; set; }

    public short? RottenTomatoesScore { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AddedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Show")]
    public virtual ICollection<ContentReport> ContentReports { get; set; } = new List<ContentReport>();

    [InverseProperty("Show")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [InverseProperty("Show")]
    public virtual ItemVector? ItemVector { get; set; }

    [InverseProperty("Show")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("ClickedShow")]
    public virtual ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();

    [InverseProperty("Show")]
    public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();

    [InverseProperty("Show")]
    public virtual ICollection<SharedListItem> SharedListItems { get; set; } = new List<SharedListItem>();

    [InverseProperty("Show")]
    public virtual ICollection<ShowAvailability> ShowAvailabilities { get; set; } = new List<ShowAvailability>();

    [InverseProperty("Show")]
    public virtual ICollection<ShowAward> ShowAwards { get; set; } = new List<ShowAward>();

    [InverseProperty("Show")]
    public virtual ICollection<UserInteraction> UserInteractions { get; set; } = new List<UserInteraction>();

    [InverseProperty("Show")]
    public virtual ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();

    [ForeignKey("ShowId")]
    [InverseProperty("Shows")]
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    [ForeignKey("ShowId")]
    [InverseProperty("Shows")]
    public virtual ICollection<ContentTag> Tags { get; set; } = new List<ContentTag>();
}
