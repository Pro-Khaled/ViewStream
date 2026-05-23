using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a single audio track, including episode context.
    /// </summary>
    public class AudioTrackDto
    {
        /// <summary>Gets or sets the unique identifier of the audio track.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode this audio track belongs to.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode this audio track belongs to.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the BCP-47 language code (e.g. "en", "ar").</summary>
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of the audio track (e.g. "dubbed", "original").</summary>
        public string? TrackType { get; set; }

        /// <summary>Gets or sets the public URL of the audio file.</summary>
        public string AudioUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this is the default audio track for the episode.</summary>
        public bool? IsDefault { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for audio tracks returned in episode track listings.
    /// </summary>
    public class AudioTrackListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the audio track.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the BCP-47 language code (e.g. "en", "ar").</summary>
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of the audio track (e.g. "dubbed", "original").</summary>
        public string? TrackType { get; set; }

        /// <summary>Gets or sets a value indicating whether this is the default audio track for the episode.</summary>
        public bool? IsDefault { get; set; }

        // required by AudioTrackMappingProfile
        /// <summary>Gets or sets a value indicating whether this record has been soft-deleted.</summary>
        public bool? IsDeleted { get; set; }

        /// <summary>Gets or sets the title of the episode this audio track belongs to.</summary>
        public string? EpisodeTitle { get; set; }
    }

    /// <summary>
    /// Request body for creating a new audio track on an episode.
    /// </summary>
    public class CreateAudioTrackDto
    {
        /// <summary>Gets or sets the ID of the episode this audio track belongs to.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the BCP-47 language code. Must be exactly 2 lowercase letters (e.g. "en").</summary>
        [Required(ErrorMessage = "LanguageCode is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "LanguageCode must be exactly 2 characters.")]
        [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "LanguageCode must be exactly 2 lowercase letters.")]
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of the audio track (e.g. "dubbed", "original"). Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "TrackType cannot exceed 20 characters.")]
        public string? TrackType { get; set; }

        /// <summary>Gets or sets the public URL of the audio file. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "AudioUrl is required.")]
        [MaxLength(500, ErrorMessage = "AudioUrl cannot exceed 500 characters.")]
        public string AudioUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this should be the default audio track for the episode.</summary>
        public bool? IsDefault { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing audio track.
    /// </summary>
    public class UpdateAudioTrackDto
    {
        /// <summary>Gets or sets the BCP-47 language code. Must be exactly 2 lowercase letters (e.g. "en").</summary>
        [Required(ErrorMessage = "LanguageCode is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "LanguageCode must be exactly 2 characters.")]
        [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "LanguageCode must be exactly 2 lowercase letters.")]
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of the audio track (e.g. "dubbed", "original"). Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "TrackType cannot exceed 20 characters.")]
        public string? TrackType { get; set; }

        /// <summary>Gets or sets the public URL of the audio file. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "AudioUrl is required.")]
        [MaxLength(500, ErrorMessage = "AudioUrl cannot exceed 500 characters.")]
        public string AudioUrl { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this should be the default audio track for the episode.</summary>
        public bool? IsDefault { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for audio tracks shown in the admin dashboard.
    /// </summary>
    public class AdminAudioTrackListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the audio track.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode this audio track belongs to.</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets the public URL of the audio file.</summary>
        public string? AudioUrl { get; set; }

        /// <summary>Gets or sets the BCP-47 language code.</summary>
        public string LanguageCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of the audio track.</summary>
        public string? TrackType { get; set; }

        /// <summary>Gets or sets a value indicating whether this is the default track for the episode.</summary>
        public bool? IsDefault { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this audio track was soft-deleted.</summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this record has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the title of the episode this audio track belongs to.</summary>
        public string? EpisodeTitle { get; set; }
    }
}
