using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a user comment on an episode.
    /// </summary>
    public class EpisodeCommentDto
    {
        /// <summary>Gets or sets the unique identifier of the comment.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode the comment is attached to.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the profile that authored the comment.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that authored the comment.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional avatar URL of the author.</summary>
        public string? ProfileAvatar { get; set; }

        /// <summary>Gets or sets the ID of the parent comment, if this is a reply.</summary>
        public long? ParentCommentId { get; set; }

        /// <summary>Gets or sets the text content of the comment.</summary>
        public string CommentText { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether the comment has been edited since creation.</summary>
        public bool? IsEdited { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the comment was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the comment was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the number of likes this comment has received.</summary>
        public int LikeCount { get; set; }

        /// <summary>Gets or sets the number of direct replies to this comment.</summary>
        public int ReplyCount { get; set; }

        /// <summary>Gets or sets the list of reply comments, if explicitly requested.</summary>
        public List<EpisodeCommentDto> Replies { get; set; } = new();
    }

    /// <summary>
    /// Slim list-item DTO for comments displayed in an episode's comment thread.
    /// </summary>
    public class EpisodeCommentListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the comment.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that authored the comment.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that authored the comment.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional avatar URL of the author.</summary>
        public string? ProfileAvatar { get; set; }

        /// <summary>Gets or sets the text content of the comment.</summary>
        public string CommentText { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether the comment has been edited.</summary>
        public bool? IsEdited { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the comment was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the number of likes this comment has received.</summary>
        public int LikeCount { get; set; }

        /// <summary>Gets or sets the number of direct replies to this comment.</summary>
        public int ReplyCount { get; set; }
    }

    /// <summary>
    /// Request body for creating a new comment or reply.
    /// </summary>
    public class CreateEpisodeCommentDto
    {
        /// <summary>Gets or sets the ID of the episode to comment on.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the ID of the parent comment, if replying to an existing comment.</summary>
        public long? ParentCommentId { get; set; }

        /// <summary>Gets or sets the text content of the comment. Maximum 2000 characters.</summary>
        [Required(ErrorMessage = "CommentText is required.")]
        [MaxLength(2000, ErrorMessage = "CommentText cannot exceed 2000 characters.")]
        public string CommentText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for editing an existing comment.
    /// </summary>
    public class UpdateEpisodeCommentDto
    {
        /// <summary>Gets or sets the updated text content of the comment. Maximum 2000 characters.</summary>
        [Required(ErrorMessage = "CommentText is required.")]
        [MaxLength(2000, ErrorMessage = "CommentText cannot exceed 2000 characters.")]
        public string CommentText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Admin list-item DTO for comments shown in the admin dashboard.
    /// </summary>
    public class AdminEpisodeCommentListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the comment.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode the comment is attached to.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the ID of the parent comment, if applicable.</summary>
        public long? ParentCommentId { get; set; }

        /// <summary>Gets or sets the ID of the profile that authored the comment.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that authored the comment.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the text content of the comment.</summary>
        public string? Content { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the comment was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the comment has been soft-deleted.</summary>
        public bool? IsDeleted { get; set; }

        /// <summary>Gets or sets a value indicating whether the comment has been edited.</summary>
        public bool? IsEdited { get; set; }

        /// <summary>Gets or sets the optional administrative moderation status.</summary>
        public string? Status { get; set; }
    }
}
