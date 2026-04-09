using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("ShowId", "AwardId")]
public partial class ShowAward
{
    [Key]
    public long ShowId { get; set; }

    [Key]
    public int AwardId { get; set; }

    public bool? Won { get; set; }

    [ForeignKey("AwardId")]
    [InverseProperty("ShowAwards")]
    public virtual Award Award { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("ShowAwards")]
    public virtual Show Show { get; set; } = null!;
}
