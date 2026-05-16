using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a user account.
    /// </summary>
    public class UserDto
    {
        /// <summary>Gets or sets the unique identifier of the user.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the user's email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's full name.</summary>
        public string? FullName { get; set; }

        /// <summary>Gets or sets the user's phone number.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Gets or sets the user's country code (e.g. "US", "GB").</summary>
        public string? CountryCode { get; set; }

        /// <summary>Gets or sets a value indicating whether the user account is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets a value indicating whether the user is blocked from signing in.</summary>
        public bool IsBlocked { get; set; }

        /// <summary>Gets or sets the optional reason for the user being blocked.</summary>
        public string? BlockedReason { get; set; }

        /// <summary>Gets or sets the UTC timestamp until which the user is blocked, if applicable.</summary>
        public DateTime? BlockedUntil { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the user registered.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the user account is soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the user was soft-deleted, if applicable.</summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>Gets or sets the roles assigned to the user.</summary>
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// Request body for a user updating their own profile details.
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>Gets or sets the user's full name. Maximum 200 characters.</summary>
        [Required(ErrorMessage = "FullName is required.")]
        [MaxLength(200, ErrorMessage = "FullName cannot exceed 200 characters.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's phone number.</summary>
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        /// <summary>Gets or sets the user's country code (e.g. "US"). Must be 2-3 characters.</summary>
        [StringLength(3, MinimumLength = 2, ErrorMessage = "CountryCode must be 2-3 characters.")]
        public string? CountryCode { get; set; }
    }

    /// <summary>
    /// Request body for a user changing their password.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>Gets or sets the user's current password.</summary>
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>Gets or sets the new password. Minimum 8 characters.</summary>
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>Gets or sets the confirmation of the new password. Must match NewPassword.</summary>
        [Required(ErrorMessage = "Confirm new password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for an admin updating a user account.
    /// </summary>
    public class AdminUpdateUserDto
    {
        /// <summary>Gets or sets the user's full name. Maximum 200 characters.</summary>
        [MaxLength(200, ErrorMessage = "FullName cannot exceed 200 characters.")]
        public string? FullName { get; set; }

        /// <summary>Gets or sets the user's phone number.</summary>
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        /// <summary>Gets or sets a value indicating whether the user account is active.</summary>
        public bool? IsActive { get; set; }

        /// <summary>Gets or sets the roles assigned to the user.</summary>
        public List<string>? Roles { get; set; }
    }

    /// <summary>
    /// Request body for an admin blocking a user account.
    /// </summary>
    public class BlockUserDto
    {
        /// <summary>Gets or sets the reason for blocking the user. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp until which the user should be blocked. If null, the block is indefinite.</summary>
        public DateTime? BlockedUntil { get; set; }
    }

    /// <summary>
    /// Safe slim DTO for the public user-search endpoint used by friend discovery.
    /// </summary>
    public class UserPublicSearchResultDto
    {
        /// <summary>Gets or sets the unique identifier of the user.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the user's full name.</summary>
        public string? FullName { get; set; }

        /// <summary>Gets or sets the user's email address (typically masked or exact match only depending on privacy rules).</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional URL of the user's primary avatar.</summary>
        public string? AvatarUrl { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for users shown in the admin dashboard.
    /// </summary>
    public class AdminUserListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the user.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the user's email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's full name.</summary>
        public string? FullName { get; set; }

        /// <summary>Gets or sets the user's phone number.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Gets or sets the user's country code.</summary>
        public string? CountryCode { get; set; }

        /// <summary>Gets or sets a value indicating whether the user account is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets a value indicating whether the user is blocked.</summary>
        public bool IsBlocked { get; set; }

        /// <summary>Gets or sets the reason the user is blocked, if applicable.</summary>
        public string? BlockedReason { get; set; }

        /// <summary>Gets or sets the UTC timestamp until which the user is blocked, if applicable.</summary>
        public DateTime? BlockedUntil { get; set; }

        /// <summary>Gets or sets a value indicating whether the user account is soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the user registered.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the user record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the number of profiles associated with this user account.</summary>
        public int ProfileCount { get; set; }

        /// <summary>Gets or sets the list of role names assigned to the user.</summary>
        public List<string> Roles { get; set; } = new();
    }
}
