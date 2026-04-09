using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("PersonId", "AwardId")]
public partial class PersonAward
{
    [Key]
    public long PersonId { get; set; }

    [Key]
    public int AwardId { get; set; }

    public bool? Won { get; set; }

    [ForeignKey("AwardId")]
    [InverseProperty("PersonAwards")]
    public virtual Award Award { get; set; } = null!;

    [ForeignKey("PersonId")]
    [InverseProperty("PersonAwards")]
    public virtual Person Person { get; set; } = null!;
}
