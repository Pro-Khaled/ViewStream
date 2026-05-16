using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    /// <summary>
    /// Request body for registering a new user account.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>Gets or sets the user's email address.</summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's password. Must be at least 8 characters.</summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>Gets or sets the confirmation password. Must match Password.</summary>
        [Required(ErrorMessage = "Confirmation password is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's full name. Maximum 200 characters.</summary>
        [Required(ErrorMessage = "FullName is required.")]
        [MaxLength(200, ErrorMessage = "FullName cannot exceed 200 characters.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's phone number.</summary>
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

        /// <summary>Gets or sets the user's country code (e.g. "US"). Must be 2 characters.</summary>
        [StringLength(2, MinimumLength = 2, ErrorMessage = "CountryCode must be exactly 2 characters.")]
        public string? CountryCode { get; set; }
    }
}
