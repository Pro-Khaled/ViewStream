using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a single episode, including show and season context.
    /// </summary>
    public class EpisodeDto
    {
        /// <summary>Gets or sets the unique identifier of the episode.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the season this episode belongs to.</summary>
        public long SeasonId { get; set; }

        /// <summary>Gets or sets the title of the show this episode belongs to.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional title of the season.</summary>
        public string? SeasonTitle { get; set; }

        /// <summary>Gets or sets the number of the season (e.g. 1, 2, 3).</summary>
        public short SeasonNumber { get; set; }

        /// <summary>Gets or sets the episode number within the season.</summary>
        public short EpisodeNumber { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional synopsis/description of the episode.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the runtime of the episode in seconds.</summary>
        public int? RuntimeSeconds { get; set; }

        /// <summary>Gets or sets the public URL of the video file.</summary>
        public string VideoUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional URL of the episode thumbnail image.</summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>Gets or sets the optional release date of the episode.</summary>
        public DateOnly? ReleaseDate { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this episode has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this episode was soft-deleted, if applicable.</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO used in season episode listings.
    /// </summary>
    public class EpisodeListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the episode.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the episode number within the season.</summary>
        public short EpisodeNumber { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the runtime of the episode in seconds.</summary>
        public int? RuntimeSeconds { get; set; }

        /// <summary>Gets or sets the URL of the episode thumbnail image.</summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>Gets or sets the release date of the episode.</summary>
        public DateOnly? ReleaseDate { get; set; }
    }

    /// <summary>
    /// Request body for creating a new episode.
    /// </summary>
    public class CreateEpisodeDto
    {
        /// <summary>Gets or sets the ID of the season this episode belongs to.</summary>
        [Required(ErrorMessage = "SeasonId is required.")]
        public long SeasonId { get; set; }

        /// <summary>Gets or sets the episode number within the season. Must be 1 or greater.</summary>
        [Required(ErrorMessage = "EpisodeNumber is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "EpisodeNumber must be at least 1.")]
        public short EpisodeNumber { get; set; }

        /// <summary>Gets or sets the title of the episode. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional synopsis/description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the runtime of the episode in seconds.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "RuntimeSeconds must be a positive value.")]
        public int? RuntimeSeconds { get; set; }

        /// <summary>Gets or sets the public URL of the video file. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "VideoUrl is required.")]
        [MaxLength(500, ErrorMessage = "VideoUrl cannot exceed 500 characters.")]
        public string VideoUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional URL of the episode thumbnail. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "ThumbnailUrl cannot exceed 500 characters.")]
        public string? ThumbnailUrl { get; set; }

        /// <summary>Gets or sets the optional release date of the episode.</summary>
        public DateOnly? ReleaseDate { get; set; }

        // Video file (uploaded separately)
        /// <summary>Gets or sets an optional video file for direct upload. Handled as multipart form data.</summary>
        public IFormFile? VideoFile { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing episode.
    /// </summary>
    public class UpdateEpisodeDto
    {
        /// <summary>Gets or sets the episode number within the season. Must be 1 or greater.</summary>
        [Required(ErrorMessage = "EpisodeNumber is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "EpisodeNumber must be at least 1.")]
        public short EpisodeNumber { get; set; }

        /// <summary>Gets or sets the title of the episode. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional synopsis/description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the runtime of the episode in seconds.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "RuntimeSeconds must be a positive value.")]
        public int? RuntimeSeconds { get; set; }

        /// <summary>Gets or sets the public URL of the video file. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "VideoUrl is required.")]
        [MaxLength(500, ErrorMessage = "VideoUrl cannot exceed 500 characters.")]
        public string VideoUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional URL of the episode thumbnail. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "ThumbnailUrl cannot exceed 500 characters.")]
        public string? ThumbnailUrl { get; set; }

        /// <summary>Gets or sets the optional release date of the episode.</summary>
        public DateOnly? ReleaseDate { get; set; }
    }

    /// <summary>
    /// DTO returned when the caller requests the stream URL for an episode.
    /// </summary>
    public class EpisodeStreamUrlDto
    {
        /// <summary>Gets or sets the public streaming URL for the episode video.</summary>
        public string VideoUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Admin list-item DTO for episodes shown in the admin dashboard.
    /// </summary>
    public class AdminEpisodeListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the episode.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the season this episode belongs to.</summary>
        public long? SeasonId { get; set; }

        /// <summary>Gets or sets the ID of the show this episode belongs to.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the episode number within the season.</summary>
        public int? EpisodeNumber { get; set; }

        /// <summary>Gets or sets the runtime of the episode in seconds.</summary>
        public int? RuntimeSeconds { get; set; }

        /// <summary>Gets or sets the URL of the episode thumbnail image.</summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>Gets or sets the optional release date of the episode.</summary>
        public DateOnly? ReleaseDate { get; set; }

        /// <summary>Gets or sets the title of the show this episode belongs to.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the season number.</summary>
        public int? SeasonNumber { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string? Title { get; set; }

        /// <summary>Gets or sets the optional synopsis/description of the episode.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the public URL of the episode video.</summary>
        public string? VideoUrl { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this episode was soft-deleted.</summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this episode has been soft-deleted.</summary>
        public bool? IsDeleted { get; set; }
    }
}
