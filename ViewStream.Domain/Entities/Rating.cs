using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("ProfileId", "ShowId")]
public partial class Rating
{
    [Key]
    public long ProfileId { get; set; }

    [Key]
    public long ShowId { get; set; }

    [Column("Rating")]
    public short Rating1 { get; set; }

    public DateTime? RatedAt { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("Ratings")]
    public virtual Profile Profile { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("Ratings")]
    public virtual Show Show { get; set; } = null!;
}
