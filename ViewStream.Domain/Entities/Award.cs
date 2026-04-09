using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Award
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Category { get; set; }

    public short? Year { get; set; }

    [InverseProperty("Award")]
    public virtual ICollection<PersonAward> PersonAwards { get; set; } = new List<PersonAward>();

    [InverseProperty("Award")]
    public virtual ICollection<ShowAward> ShowAwards { get; set; } = new List<ShowAward>();
}
