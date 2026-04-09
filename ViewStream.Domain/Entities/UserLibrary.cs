using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Table("UserLibrary")]
[Index("ProfileId", "Status", Name = "IX_UserLibrary_ProfileId_Status")]
[Index("ProfileId", "ShowId", "SeasonId", Name = "UQ_UserLibrary_ProfileId_Target", IsUnique = true)]
public partial class UserLibrary
{
    [Key]
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long? ShowId { get; set; }

    public long? SeasonId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public int? EpisodesWatched { get; set; }

    [Column(TypeName = "decimal(2, 1)")]
    public decimal? UserScore { get; set; }

    public DateOnly? StartedAt { get; set; }

    public DateOnly? CompletedAt { get; set; }

    public DateTime? AddedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("UserLibraries")]
    public virtual Profile Profile { get; set; } = null!;

    [ForeignKey("SeasonId")]
    [InverseProperty("UserLibraries")]
    public virtual Season? Season { get; set; }

    [ForeignKey("ShowId")]
    [InverseProperty("UserLibraries")]
    public virtual Show? Show { get; set; }
}
