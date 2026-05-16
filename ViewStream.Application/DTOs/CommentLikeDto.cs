using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a reaction or "like" on an episode comment.
    /// </summary>
    public class CommentLikeDto
    {
        /// <summary>Gets or sets the ID of the comment reacted to.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the ID of the profile that reacted.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of reaction (e.g. "like", "dislike").</summary>
        public string? ReactionType { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the reaction was recorded.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Aggregated summary of reactions for a specific comment.
    /// </summary>
    public class CommentReactionSummaryDto
    {
        /// <summary>Gets or sets the ID of the comment.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the total number of reactions on the comment.</summary>
        public int TotalReactions { get; set; }

        /// <summary>Gets or sets a dictionary mapping reaction types to their counts.</summary>
        public Dictionary<string, int> ReactionCounts { get; set; } = new();

        /// <summary>Gets or sets the current user's reaction, if any.</summary>
        public string? CurrentUserReaction { get; set; }
    }

    /// <summary>
    /// Request body for adding or updating a reaction to a comment.
    /// </summary>
    public class CreateUpdateCommentLikeDto
    {
        /// <summary>Gets or sets the ID of the comment to react to.</summary>
        [Required(ErrorMessage = "CommentId is required.")]
        public long CommentId { get; set; }

        /// <summary>Gets or sets the type of reaction. Defaults to "like". Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "ReactionType cannot exceed 20 characters.")]
        public string? ReactionType { get; set; } = "like";
    }

    /// <summary>
    /// Slim list-item DTO for viewing individual reactions on a comment.
    /// </summary>
    public class CommentLikeListItemDto
    {
        /// <summary>Gets or sets the ID of the comment reacted to.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the ID of the profile that reacted.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the type of reaction.</summary>
        public string? ReactionType { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the reaction was recorded.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for comment reactions shown in the admin dashboard.
    /// </summary>
    public class AdminCommentLikeListItemDto
    {
        /// <summary>Gets or sets the ID of the comment reacted to.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the ID of the profile that reacted.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the type of reaction.</summary>
        public string? ReactionType { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the reaction was recorded.</summary>
        public DateTime? CreatedAt { get; set; }
    }
}
