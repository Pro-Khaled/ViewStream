using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViewStream.Domain.Entities;

/// <summary>
/// Represents a user appeal against a moderation decision (hide, ban, suspend).
/// </summary>
public partial class Appeal
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    /// <summary>e.g. "Comment", "Episode", "Ban"</summary>
    [StringLength(30)]
    public string EntityType { get; set; } = null!;

    /// <summary>The ID of the hidden comment/episode, or 0 for ban appeals.</summary>
    public long EntityId { get; set; }

    [StringLength(2000)]
    public string Reason { get; set; } = null!;

    /// <summary>pending, approved, rejected</summary>
    [StringLength(20)]
    public string Status { get; set; } = "pending";

    public DateTime? ReviewedAt { get; set; }

    public long? ReviewedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ReviewedByUserId")]
    public virtual User? ReviewedByUser { get; set; }
}
