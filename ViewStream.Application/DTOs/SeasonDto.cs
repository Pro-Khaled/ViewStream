using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a single season, including show context and episode count.
    /// </summary>
    public class SeasonDto
    {
        /// <summary>Gets or sets the unique identifier of the season.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the show this season belongs to.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show this season belongs to.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the season number (e.g. 1, 2, 3).</summary>
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the optional title of the season.</summary>
        public string? Title { get; set; }

        /// <summary>Gets or sets the optional synopsis/description of the season.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the optional release date of the season.</summary>
        public DateOnly? ReleaseDate { get; set; }

        /// <summary>Gets or sets the number of episodes in this season.</summary>
        public int EpisodeCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this season has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this season was soft-deleted, if applicable.</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO used in show season listings.
    /// </summary>
    public class SeasonListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the season.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the season number.</summary>
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the optional title of the season.</summary>
        public string? Title { get; set; }

        /// <summary>Gets or sets the optional release date of the season.</summary>
        public DateOnly? ReleaseDate { get; set; }

        /// <summary>Gets or sets the number of episodes in this season.</summary>
        public int EpisodeCount { get; set; }
    }

    /// <summary>
    /// Request body for creating a new season.
    /// </summary>
    public class CreateSeasonDto
    {
        /// <summary>Gets or sets the ID of the show this season belongs to.</summary>
        [Required(ErrorMessage = "ShowId is required.")]
        public long ShowId { get; set; }

        /// <summary>Gets or sets the season number. Must be 1 or greater.</summary>
        [Required(ErrorMessage = "SeasonNumber is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "SeasonNumber must be at least 1.")]
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the optional title of the season. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters.")]
        public string? Title { get; set; }

        /// <summary>Gets or sets the optional synopsis/description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the optional release date of the season.</summary>
        public DateOnly? ReleaseDate { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing season.
    /// </summary>
    public class UpdateSeasonDto
    {
        /// <summary>Gets or sets the season number. Must be 1 or greater.</summary>
        [Required(ErrorMessage = "SeasonNumber is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "SeasonNumber must be at least 1.")]
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the optional title of the season. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters.")]
        public string? Title { get; set; }

        /// <summary>Gets or sets the optional synopsis/description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the optional release date of the season.</summary>
        public DateOnly? ReleaseDate { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for seasons shown in the admin dashboard.
    /// </summary>
    public class AdminSeasonListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the season.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the show this season belongs to.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the season number.</summary>
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the optional title of the season.</summary>
        public string? Title { get; set; }

        /// <summary>Gets or sets the optional synopsis/description of the season.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the optional release date of the season.</summary>
        public DateOnly? ReleaseDate { get; set; }

        /// <summary>Gets or sets a value indicating whether this season has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this season was soft-deleted, if applicable.</summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>Gets or sets the title of the show this season belongs to.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of episodes in this season.</summary>
        public int EpisodeCount { get; set; }
    }
}
