using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class UserInteraction
{
    [Key]
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long ShowId { get; set; }

    [StringLength(20)]
    public string InteractionType { get; set; } = null!;

    [Column(TypeName = "decimal(5, 3)")]
    public decimal? Weight { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("UserInteractions")]
    public virtual Profile Profile { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("UserInteractions")]
    public virtual Show Show { get; set; } = null!;
}
