using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a single viewing history record.
    /// </summary>
    public class WatchHistoryDto
    {
        /// <summary>Gets or sets the unique identifier of the history record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that watched the episode.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the episode watched.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode watched.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the season the episode belongs to.</summary>
        public long SeasonId { get; set; }

        /// <summary>Gets or sets the season number.</summary>
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the ID of the show the episode belongs to.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the URL of the show poster.</summary>
        public string? ShowPosterUrl { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the episode was last watched.</summary>
        public DateTime? WatchedAt { get; set; }

        /// <summary>Gets or sets the viewing progress in seconds.</summary>
        public int? ProgressSeconds { get; set; }

        /// <summary>Gets or sets the total duration of the episode in seconds.</summary>
        public int? TotalSeconds { get; set; }

        /// <summary>Gets or sets a value indicating whether the episode was watched to completion.</summary>
        public bool? Completed { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for viewing history displayed in the "Continue Watching" row.
    /// </summary>
    public class WatchHistoryListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the history record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode watched.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the URL of the show poster.</summary>
        public string? ShowPosterUrl { get; set; }

        /// <summary>Gets or sets the viewing progress in seconds.</summary>
        public int? ProgressSeconds { get; set; }

        /// <summary>Gets or sets the total duration of the episode in seconds.</summary>
        public int? TotalSeconds { get; set; }

        /// <summary>Gets or sets a value indicating whether the episode was watched to completion.</summary>
        public bool? Completed { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the episode was last watched.</summary>
        public DateTime? WatchedAt { get; set; }
    }

    /// <summary>
    /// Request body for syncing playback progress periodically.
    /// </summary>
    public class CreateUpdateWatchHistoryDto
    {
        /// <summary>Gets or sets the ID of the episode currently playing.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the current playback position in seconds.</summary>
        [Range(0, int.MaxValue, ErrorMessage = "ProgressSeconds cannot be negative.")]
        public int? ProgressSeconds { get; set; }

        /// <summary>Gets or sets a value indicating whether the episode has reached the end credits or completion threshold.</summary>
        public bool? Completed { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for viewing history shown in the admin dashboard.
    /// </summary>
    public class AdminWatchHistoryListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the history record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that watched the episode.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the ID of the episode watched.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the ID of the user owning the profile.</summary>
        public long? UserId { get; set; }

        /// <summary>Gets or sets a value indicating whether the episode was watched to completion.</summary>
        public bool? Completed { get; set; }

        /// <summary>Gets or sets the viewing progress in seconds.</summary>
        public int? ProgressSeconds { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the episode was last watched.</summary>
        public DateTime? WatchedAt { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public string ShowTitle { get; set; } = string.Empty;
        public int? TotalSeconds { get; set; }
        public string? ShowPosterUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

