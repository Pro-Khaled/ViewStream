using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of an item saved in a user's personal library/watchlist.
    /// </summary>
    public class UserLibraryDto
    {
        /// <summary>Gets or sets the unique identifier of the library entry.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile owning this entry.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional ID of the show saved.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the URL of the show poster.</summary>
        public string? ShowPosterUrl { get; set; }

        /// <summary>Gets or sets the optional ID of a specific season saved.</summary>
        public long? SeasonId { get; set; }

        /// <summary>Gets or sets the title of the season.</summary>
        public string? SeasonTitle { get; set; }

        /// <summary>Gets or sets the season number.</summary>
        public short? SeasonNumber { get; set; }

        /// <summary>Gets or sets the tracking status (e.g. "plan_to_watch", "watching", "completed", "dropped").</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of episodes the user has watched so far.</summary>
        public int? EpisodesWatched { get; set; }

        /// <summary>Gets or sets the user's personal rating score.</summary>
        public decimal? UserScore { get; set; }

        /// <summary>Gets or sets the date the user started watching.</summary>
        public DateOnly? StartedAt { get; set; }

        /// <summary>Gets or sets the date the user finished watching.</summary>
        public DateOnly? CompletedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the item was added to the library.</summary>
        public DateTime? AddedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the entry was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for library entries displayed in a grid.
    /// </summary>
    public class UserLibraryListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the library entry.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the optional ID of the show saved.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the title of the saved item.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the URL of the poster image.</summary>
        public string? PosterUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a "Show" or "Season".</summary>
        public string ItemType { get; set; } = "Show";

        /// <summary>Gets or sets the tracking status.</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of episodes the user has watched so far.</summary>
        public int? EpisodesWatched { get; set; }

        /// <summary>Gets or sets the total number of available episodes for the item.</summary>
        public int? TotalEpisodes { get; set; }

        /// <summary>Gets or sets the user's personal rating score.</summary>
        public decimal? UserScore { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the item was added to the library.</summary>
        public DateTime? AddedAt { get; set; }
    }

    /// <summary>
    /// Request body for adding an item to the user's library.
    /// </summary>
    public class CreateUserLibraryDto
    {
        /// <summary>Gets or sets the optional ID of the show to add.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the optional ID of a specific season to add.</summary>
        public long? SeasonId { get; set; }

        /// <summary>Gets or sets the tracking status. Must be a valid tracking string.</summary>
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; } = "plan_to_watch";

        /// <summary>Gets or sets the initial number of episodes watched.</summary>
        [Range(0, int.MaxValue, ErrorMessage = "EpisodesWatched cannot be negative.")]
        public int? EpisodesWatched { get; set; }

        /// <summary>Gets or sets the initial personal rating score (e.g. 1.0 to 10.0).</summary>
        public decimal? UserScore { get; set; }

        /// <summary>Gets or sets the date the user started watching.</summary>
        public DateOnly? StartedAt { get; set; }

        /// <summary>Gets or sets the date the user finished watching.</summary>
        public DateOnly? CompletedAt { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing library entry.
    /// </summary>
    public class UpdateUserLibraryDto
    {
        /// <summary>Gets or sets the updated tracking status.</summary>
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string? Status { get; set; }

        /// <summary>Gets or sets the updated number of episodes watched.</summary>
        [Range(0, int.MaxValue, ErrorMessage = "EpisodesWatched cannot be negative.")]
        public int? EpisodesWatched { get; set; }

        /// <summary>Gets or sets the updated personal rating score.</summary>
        public decimal? UserScore { get; set; }

        /// <summary>Gets or sets the updated date the user started watching.</summary>
        public DateOnly? StartedAt { get; set; }

        /// <summary>Gets or sets the updated date the user finished watching.</summary>
        public DateOnly? CompletedAt { get; set; }
    }

    /// <summary>
    /// Aggregate summary DTO of a profile's library statistics.
    /// </summary>
    public class UserLibrarySummaryDto
    {
        /// <summary>Gets or sets the ID of the profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the total number of items in the library.</summary>
        public int TotalItems { get; set; }

        /// <summary>Gets or sets the dictionary mapping each status to the count of items.</summary>
        public Dictionary<string, int> CountByStatus { get; set; } = new();
    }

    /// <summary>
    /// Admin list-item DTO for library entries shown in the admin dashboard.
    /// </summary>
    public class AdminUserLibraryListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the library entry.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile owning this entry.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the profile.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the ID of the show saved, if applicable.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the number of episodes watched.</summary>
        public int? EpisodesWatched { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the item was added.</summary>
        public DateTime? AddedAt { get; set; }
        public long? SeasonId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? UserScore { get; set; }
        public DateOnly? StartedAt { get; set; }
        public DateOnly? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

