using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("ProfileId", "RowName")]
public partial class PersonalizedRow
{
    [Key]
    public long ProfileId { get; set; }

    [Key]
    [StringLength(50)]
    public string RowName { get; set; } = null!;

    public string ShowIdsJson { get; set; } = null!;

    public DateTime? GeneratedAt { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("PersonalizedRows")]
    public virtual Profile Profile { get; set; } = null!;
}
