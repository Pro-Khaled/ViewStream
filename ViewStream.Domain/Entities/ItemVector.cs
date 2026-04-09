using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class ItemVector
{
    [Key]
    public long ShowId { get; set; }

    public string? EmbeddingJson { get; set; }

    public DateTime? LastUpdated { get; set; }

    [ForeignKey("ShowId")]
    [InverseProperty("ItemVector")]
    public virtual Show Show { get; set; } = null!;
}
