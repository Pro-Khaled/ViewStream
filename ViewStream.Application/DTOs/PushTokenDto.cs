using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a push notification device token.
    /// </summary>
    public class PushTokenDto
    {
        /// <summary>Gets or sets the unique identifier of the push token record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the device.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the hardware or tracking ID of the device.</summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the push notification token provided by the OS (e.g. APNs, FCM).</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Gets or sets the operating system or platform (e.g. "iOS", "Android").</summary>
        public string? Platform { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the token was last used to send a notification.</summary>
        public DateTime? LastUsed { get; set; }
    }

    /// <summary>
    /// Request body for registering a new push notification token from a client device.
    /// </summary>
    public class CreatePushTokenDto
    {
        /// <summary>Gets or sets the hardware or tracking ID of the device. Maximum 255 characters.</summary>
        [Required(ErrorMessage = "DeviceId is required.")]
        [MaxLength(255, ErrorMessage = "DeviceId cannot exceed 255 characters.")]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the push notification token. Maximum 1000 characters.</summary>
        [Required(ErrorMessage = "Token is required.")]
        [MaxLength(1000, ErrorMessage = "Token cannot exceed 1000 characters.")]
        public string Token { get; set; } = string.Empty;

        /// <summary>Gets or sets the operating system or platform. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "Platform cannot exceed 50 characters.")]
        public string? Platform { get; set; }
    }
}
