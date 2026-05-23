using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an active or expired login session for a user.
    /// </summary>
    public class LoginSessionDto
    {
        /// <summary>Gets or sets the unique identifier of the login session.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the session.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the optional ID of the device associated with this session.</summary>
        public long? DeviceId { get; set; }

        /// <summary>Gets or sets the user-friendly name of the device.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the opaque session token or refresh token.</summary>
        public string SessionToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the IP address from which the session was initiated.</summary>
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the user agent string of the client.</summary>
        public string? UserAgent { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session is scheduled to expire.</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session was manually revoked, if applicable.</summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the session is currently active and valid.</summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user viewing their active sessions.
    /// </summary>
    public class LoginSessionListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the login session.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the user-friendly name of the device.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the IP address from which the session was initiated.</summary>
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session is scheduled to expire.</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the session is active.</summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for login sessions shown in the admin dashboard.
    /// </summary>
    public class AdminLoginSessionListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the login session.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the session.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the user-friendly name of the device.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the unique hardware or tracking ID of the device.</summary>
        public string? DeviceId { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the session was last used.</summary>
        public DateTime? LastActive { get; set; }

        /// <summary>Gets or sets a value indicating whether the session is currently active.</summary>
        public bool? IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}

