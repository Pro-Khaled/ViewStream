using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("ProfileId", Name = "IX_EpisodeComments_ProfileId")]
public partial class EpisodeComment
{
    [Key]
    public long Id { get; set; }

    public long EpisodeId { get; set; }

    public long ProfileId { get; set; }

    public long? ParentCommentId { get; set; }

    [StringLength(2000)]
    public string CommentText { get; set; } = null!;

    public bool? IsEdited { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Comment")]
    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    [InverseProperty("Comment")]
    public virtual ICollection<CommentReport> CommentReports { get; set; } = new List<CommentReport>();

    [ForeignKey("EpisodeId")]
    [InverseProperty("EpisodeComments")]
    public virtual Episode Episode { get; set; } = null!;

    [InverseProperty("ParentComment")]
    public virtual ICollection<EpisodeComment> InverseParentComment { get; set; } = new List<EpisodeComment>();

    [ForeignKey("ParentCommentId")]
    [InverseProperty("InverseParentComment")]
    public virtual EpisodeComment? ParentComment { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("EpisodeComments")]
    public virtual Profile Profile { get; set; } = null!;
}
