using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a subtitle file associated with an episode.
    /// </summary>
    public class SubtitleDto
    {
        /// <summary>Gets or sets the unique identifier of the subtitle.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode this subtitle belongs to.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode this subtitle belongs to.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the BCP-47 language code (e.g. "en", "es").</summary>
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the public URL of the subtitle file (e.g. .vtt).</summary>
        public string SubtitleUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this subtitle contains closed captions (CC).</summary>
        public bool? IsCc { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO used in the video player track selector.
    /// </summary>
    public class SubtitleListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the subtitle.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the BCP-47 language code (e.g. "en", "es").</summary>
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the public URL of the subtitle file.</summary>
        public string SubtitleUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this subtitle contains closed captions (CC).</summary>
        public bool? IsCc { get; set; }
    }

    /// <summary>
    /// Request body for creating a new subtitle record.
    /// </summary>
    public class CreateSubtitleDto
    {
        /// <summary>Gets or sets the ID of the episode this subtitle belongs to.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the BCP-47 language code. Must be exactly 2 lowercase letters (e.g. "en").</summary>
        [Required(ErrorMessage = "LanguageCode is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "LanguageCode must be exactly 2 characters.")]
        [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "LanguageCode must be exactly 2 lowercase letters.")]
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the public URL of the subtitle file. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "SubtitleUrl is required.")]
        [MaxLength(500, ErrorMessage = "SubtitleUrl cannot exceed 500 characters.")]
        public string SubtitleUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this subtitle contains closed captions.</summary>
        public bool? IsCc { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing subtitle record.
    /// </summary>
    public class UpdateSubtitleDto
    {
        /// <summary>Gets or sets the BCP-47 language code. Must be exactly 2 lowercase letters (e.g. "en").</summary>
        [Required(ErrorMessage = "LanguageCode is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "LanguageCode must be exactly 2 characters.")]
        [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "LanguageCode must be exactly 2 lowercase letters.")]
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the public URL of the subtitle file. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "SubtitleUrl is required.")]
        [MaxLength(500, ErrorMessage = "SubtitleUrl cannot exceed 500 characters.")]
        public string SubtitleUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this subtitle contains closed captions.</summary>
        public bool? IsCc { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for subtitles shown in the admin dashboard.
    /// </summary>
    public class AdminSubtitleListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the subtitle.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the BCP-47 language code.</summary>
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the public URL of the subtitle file.</summary>
        public string SubtitleUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this subtitle contains closed captions.</summary>
        public bool? IsCc { get; set; }

        /// <summary>Gets or sets a value indicating whether this subtitle record has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the title of the episode this subtitle belongs to.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;
        public long EpisodeId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

