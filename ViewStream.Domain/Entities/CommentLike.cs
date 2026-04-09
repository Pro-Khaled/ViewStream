using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("CommentId", "ProfileId")]
public partial class CommentLike
{
    [Key]
    public long CommentId { get; set; }

    [Key]
    public long ProfileId { get; set; }

    [StringLength(20)]
    public string? ReactionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CommentId")]
    [InverseProperty("CommentLikes")]
    public virtual EpisodeComment Comment { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [InverseProperty("CommentLikes")]
    public virtual Profile Profile { get; set; } = null!;
}
