using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an episode downloaded for offline viewing.
    /// </summary>
    public class OfflineDownloadDto
    {
        /// <summary>Gets or sets the unique identifier of the download record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that initiated the download.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the ID of the episode downloaded.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode downloaded.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the device where the file is stored.</summary>
        public long DeviceId { get; set; }

        /// <summary>Gets or sets the user-friendly name of the device.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the video quality of the download (e.g. "720p", "1080p").</summary>
        public string? DownloadQuality { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the download completed.</summary>
        public DateTime? DownloadedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the download license expires.</summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>Gets or sets the local encrypted file path on the client device.</summary>
        public string? FilePath { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user viewing their downloaded episodes.
    /// </summary>
    public class OfflineDownloadListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the download record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the episode downloaded.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the user-friendly name of the device.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the video quality of the download.</summary>
        public string? DownloadQuality { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the download completed.</summary>
        public DateTime? DownloadedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the download license expires.</summary>
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// Request body for registering a new offline download.
    /// </summary>
    public class CreateOfflineDownloadDto
    {
        /// <summary>Gets or sets the ID of the episode being downloaded.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the ID of the device storing the download.</summary>
        [Required(ErrorMessage = "DeviceId is required.")]
        public long DeviceId { get; set; }

        /// <summary>Gets or sets the video quality. Maximum 20 characters.</summary>
        [MaxLength(20, ErrorMessage = "DownloadQuality cannot exceed 20 characters.")]
        public string? DownloadQuality { get; set; }

        /// <summary>Gets or sets the local encrypted file path on the client. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "FilePath cannot exceed 500 characters.")]
        public string? FilePath { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the download license expires.</summary>
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for offline downloads shown in the admin dashboard.
    /// </summary>
    public class AdminOfflineDownloadListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the download record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the device.</summary>
        public long DeviceId { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the device.</summary>
        public long DeviceUserId { get; set; }

        /// <summary>Gets or sets the ID of the episode.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the ID of the profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the download completed.</summary>
        public DateTime? DownloadedAt { get; set; }
    }
}
