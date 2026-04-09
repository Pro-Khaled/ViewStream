using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Person
{
    [Key]
    public long Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Bio { get; set; }

    [StringLength(500)]
    public string? PhotoUrl { get; set; }

    [InverseProperty("Person")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [InverseProperty("Person")]
    public virtual ICollection<PersonAward> PersonAwards { get; set; } = new List<PersonAward>();
}
