using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a moderation report submitted against a user comment.
    /// </summary>
    public class CommentReportDto
    {
        /// <summary>Gets or sets the unique identifier of the report.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the comment being reported.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the text of the reported comment.</summary>
        public string CommentText { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name of the comment's author.</summary>
        public string CommentAuthorName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the profile that submitted the report.</summary>
        public long ReportedByProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that submitted the report.</summary>
        public string ReportedByProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary reason for the report (e.g. "Spam", "Harassment").</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets optional additional details provided by the reporter.</summary>
        public string? Details { get; set; }

        /// <summary>Gets or sets the current moderation status of the report (e.g. "pending", "reviewed", "action_taken").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the ID of the admin user who reviewed the report.</summary>
        public long? ReviewedByUserId { get; set; }

        /// <summary>Gets or sets the display name or email of the admin who reviewed the report.</summary>
        public string? ReviewedByUserName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was reviewed.</summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was submitted.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a comment report.
    /// </summary>
    public class CommentReportListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the report.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the comment being reported.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the text of the reported comment.</summary>
        public string CommentText { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name of the profile that submitted the report.</summary>
        public string ReportedByProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary reason for the report.</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets the current moderation status of the report.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was submitted.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for submitting a new report against a comment.
    /// </summary>
    public class CreateCommentReportDto
    {
        /// <summary>Gets or sets the ID of the comment being reported.</summary>
        [Required(ErrorMessage = "CommentId is required.")]
        public long CommentId { get; set; }

        /// <summary>Gets or sets the primary reason for the report. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(100, ErrorMessage = "Reason cannot exceed 100 characters.")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets optional additional details. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Details cannot exceed 500 characters.")]
        public string? Details { get; set; }
    }

    /// <summary>
    /// Request body for an admin to update the moderation status of a report.
    /// </summary>
    public class UpdateReportStatusDto
    {
        /// <summary>Gets or sets the updated moderation status (e.g. "reviewed", "dismissed", "action_taken").</summary>
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Admin list-item DTO for comment reports shown in the admin dashboard.
    /// </summary>
    public class AdminCommentReportListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the report.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the comment being reported.</summary>
        public long CommentId { get; set; }

        /// <summary>Gets or sets the text of the reported comment.</summary>
        public string? CommentText { get; set; }

        /// <summary>Gets or sets the text details of the report (optional).</summary>
        public string? Details { get; set; }

        /// <summary>Gets or sets the display name of the profile that submitted the report.</summary>
        public string? ReportedByProfileName { get; set; }

        /// <summary>Gets or sets the primary reason for the report.</summary>
        public string? Reason { get; set; }

        /// <summary>Gets or sets the current moderation status.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the ID of the admin user who reviewed the report.</summary>
        public long? ReviewedByUserId { get; set; }

        /// <summary>Gets or sets the display name of the admin who reviewed the report.</summary>
        public string? ReviewedByUserName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was reviewed.</summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was submitted.</summary>
        public DateTime? CreatedAt { get; set; }

        // Soft-delete fields are not part of CommentReport entity, so they are intentionally omitted.
    }
}
