using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("ShowId", "SeasonNumber", Name = "UQ_Seasons_ShowId_SeasonNumber", IsUnique = true)]
public partial class Season
{
    [Key]
    public long Id { get; set; }

    public long ShowId { get; set; }

    public short SeasonNumber { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Season")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [InverseProperty("Season")]
    public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    [ForeignKey("ShowId")]
    [InverseProperty("Seasons")]
    public virtual Show Show { get; set; } = null!;

    [InverseProperty("Season")]
    public virtual ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();
}
