using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a raw telemetry event tracked during video playback.
    /// </summary>
    public class PlaybackEventDto
    {
        /// <summary>Gets or sets the unique identifier of the event.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile playing the video.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the ID of the episode being played.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the type of event (e.g. "play", "pause", "buffer", "seek").</summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>Gets or sets the playback position in seconds when the event occurred.</summary>
        public int? PositionSeconds { get; set; }

        /// <summary>Gets or sets the video quality being streamed (e.g. "1080p").</summary>
        public string? Quality { get; set; }

        /// <summary>Gets or sets the current bitrate in Kbps.</summary>
        public int? BitrateKbps { get; set; }

        /// <summary>Gets or sets the cumulative number of buffering events in this session.</summary>
        public int? BufferingCount { get; set; }

        /// <summary>Gets or sets the type of device playing the video (e.g. "Web", "iOS", "Android").</summary>
        public string? DeviceType { get; set; }

        /// <summary>Gets or sets the IP address of the client.</summary>
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the user agent string of the client.</summary>
        public string? UserAgent { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the event was recorded.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for submitting a new playback telemetry event from a client.
    /// </summary>
    public class CreatePlaybackEventDto
    {
        /// <summary>Gets or sets the ID of the episode being played.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the type of event. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "EventType is required.")]
        [MaxLength(50, ErrorMessage = "EventType cannot exceed 50 characters.")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>Gets or sets the playback position in seconds.</summary>
        [Range(0, int.MaxValue, ErrorMessage = "PositionSeconds cannot be negative.")]
        public int? PositionSeconds { get; set; }

        /// <summary>Gets or sets the video quality. Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "Quality cannot exceed 20 characters.")]
        public string? Quality { get; set; }

        /// <summary>Gets or sets the current bitrate in Kbps.</summary>
        [Range(0, int.MaxValue, ErrorMessage = "BitrateKbps cannot be negative.")]
        public int? BitrateKbps { get; set; }

        /// <summary>Gets or sets the cumulative number of buffering events.</summary>
        [Range(0, int.MaxValue, ErrorMessage = "BufferingCount cannot be negative.")]
        public int? BufferingCount { get; set; }

        /// <summary>Gets or sets the device type. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "DeviceType cannot exceed 50 characters.")]
        public string? DeviceType { get; set; }

        /// <summary>Gets or sets the IP address. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "IpAddress cannot exceed 50 characters.")]
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the user agent string. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "UserAgent cannot exceed 500 characters.")]
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for playback events shown in the admin dashboard.
    /// </summary>
    public class AdminPlaybackEventListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the event.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile playing the video.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the ID of the episode.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the type of event.</summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>Gets or sets the playback position in seconds.</summary>
        public int? PositionSeconds { get; set; }

        /// <summary>Gets or sets the video quality.</summary>
        public string? Quality { get; set; }

        /// <summary>Gets or sets the current bitrate in Kbps.</summary>
        public int? BitrateKbps { get; set; }

        /// <summary>Gets or sets the cumulative number of buffering events in this session.</summary>
        public int? BufferingCount { get; set; }

        /// <summary>Gets or sets the type of device playing the video.</summary>
        public string? DeviceType { get; set; }

        /// <summary>Gets or sets the IP address of the client.</summary>
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the user agent string of the client.</summary>
        public string? UserAgent { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the event was recorded.</summary>
        public DateTime? CreatedAt { get; set; }

    }
}

