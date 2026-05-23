using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class UserVector
{
    [Key]
    public long ProfileId { get; set; }

    public string? EmbeddingJson { get; set; }

    public DateTime? LastUpdated { get; set; }

    /// <summary>UTC timestamp when the vector was first generated.</summary>
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("UserVector")]
    public virtual Profile Profile { get; set; } = null!;
}
