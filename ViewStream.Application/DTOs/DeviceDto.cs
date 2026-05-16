using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a registered device used by a user to access the platform.
    /// </summary>
    public class DeviceDto
    {
        /// <summary>Gets or sets the unique identifier of the device record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the device.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the unique hardware or tracking ID of the device.</summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the user-friendly name of the device (e.g. "My iPhone").</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the operating system or platform (e.g. "iOS", "Android", "Web").</summary>
        public string? Platform { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the device was last active.</summary>
        public DateTime? LastActive { get; set; }

        /// <summary>Gets or sets a value indicating whether the device is marked as trusted.</summary>
        public bool? IsTrusted { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user viewing their own devices.
    /// </summary>
    public class DeviceListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the device record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the user-friendly name of the device.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the platform of the device.</summary>
        public string? Platform { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the device was last active.</summary>
        public DateTime? LastActive { get; set; }

        /// <summary>Gets or sets a value indicating whether the device is marked as trusted.</summary>
        public bool? IsTrusted { get; set; }
    }

    /// <summary>
    /// Request body for registering a new device.
    /// </summary>
    public class CreateDeviceDto
    {
        /// <summary>Gets or sets the unique hardware or tracking ID. Maximum 255 characters.</summary>
        [Required(ErrorMessage = "DeviceId is required.")]
        [MaxLength(255, ErrorMessage = "DeviceId cannot exceed 255 characters.")]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the user-friendly name. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "DeviceName cannot exceed 100 characters.")]
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the platform. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "Platform cannot exceed 50 characters.")]
        public string? Platform { get; set; }
    }

    /// <summary>
    /// Request body for updating device preferences.
    /// </summary>
    public class UpdateDeviceDto
    {
        /// <summary>Gets or sets the updated user-friendly name. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "DeviceName cannot exceed 100 characters.")]
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the updated trusted status.</summary>
        public bool? IsTrusted { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for devices shown in the admin dashboard.
    /// </summary>
    public class AdminDeviceListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the device record.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user who owns the device.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the unique hardware or tracking ID.</summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the user-friendly name.</summary>
        public string? DeviceName { get; set; }

        /// <summary>Gets or sets the platform.</summary>
        public string? Platform { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the device was last active.</summary>
        public DateTime? LastActive { get; set; }

        /// <summary>Gets or sets a value indicating whether the device is trusted.</summary>
        public bool? IsTrusted { get; set; }
    }
}
