using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a moderation report submitted against video content (Show or Episode).
    /// </summary>
    public class ContentReportDto
    {
        /// <summary>Gets or sets the unique identifier of the report.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that submitted the report.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that submitted the report.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional ID of the reported show.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the title of the reported show.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the optional ID of the reported episode.</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the reported episode.</summary>
        public string? EpisodeTitle { get; set; }

        /// <summary>Gets or sets the primary reason for the report (e.g. "Playback Issue", "Inappropriate Content").</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets optional additional details describing the issue.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the current moderation status of the report (e.g. "pending", "resolved", "dismissed").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was submitted.</summary>
        public DateTime? ReportedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was marked resolved or dismissed.</summary>
        public DateTime? ResolvedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a content report.
    /// </summary>
    public class ContentReportListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the report.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile that submitted the report.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of target reported ("Show" or "Episode").</summary>
        public string TargetType { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the reported content.</summary>
        public string TargetTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary reason for the report.</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets the current moderation status.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was submitted.</summary>
        public DateTime? ReportedAt { get; set; }
    }

    /// <summary>
    /// Request body for submitting a new report against content.
    /// </summary>
    public class CreateContentReportDto
    {
        /// <summary>Gets or sets the optional ID of the show being reported.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the optional ID of the episode being reported.</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets the primary reason for the report. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(100, ErrorMessage = "Reason cannot exceed 100 characters.")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets optional additional details. Maximum 1000 characters.</summary>
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request body for an admin to update the moderation status of a content report.
    /// </summary>
    public class UpdateContentReportStatusDto
    {
        /// <summary>Gets or sets the updated moderation status (e.g. "pending", "reviewed", "dismissed", "action_taken").</summary>
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Admin list-item DTO for content reports shown in the admin dashboard.
    /// </summary>
    public class AdminContentReportListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the report.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile that submitted the report.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the type of target reported ("Show" or "Episode").</summary>
        public string? TargetType { get; set; }

        /// <summary>Gets or sets the title of the reported content.</summary>
        public string? TargetTitle { get; set; }

        /// <summary>Gets or sets the primary reason for the report.</summary>
        public string? Reason { get; set; }

        /// <summary>Gets or sets the current moderation status.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the report was submitted.</summary>
        public DateTime? ReportedAt { get; set; }
    }
}
