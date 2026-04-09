using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Credit
{
    [Key]
    public long Id { get; set; }

    public long PersonId { get; set; }

    public long? ShowId { get; set; }

    public long? SeasonId { get; set; }

    public long? EpisodeId { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = null!;

    [StringLength(100)]
    public string? CharacterName { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("Credits")]
    public virtual Episode? Episode { get; set; }

    [ForeignKey("PersonId")]
    [InverseProperty("Credits")]
    public virtual Person Person { get; set; } = null!;

    [ForeignKey("SeasonId")]
    [InverseProperty("Credits")]
    public virtual Season? Season { get; set; }

    [ForeignKey("ShowId")]
    [InverseProperty("Credits")]
    public virtual Show? Show { get; set; }
}
