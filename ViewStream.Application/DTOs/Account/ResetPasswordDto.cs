using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    /// <summary>
    /// Request body for completing a password reset using a token.
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>Gets or sets the ID of the user resetting their password.</summary>
        [Required(ErrorMessage = "UserId is required.")]
        public long UserId { get; set; }

        /// <summary>Gets or sets the secure reset token provided to the user.</summary>
        [Required(ErrorMessage = "Reset token is required.")]
        public string Token { get; set; } = string.Empty;

        /// <summary>Gets or sets the new password. Minimum 8 characters.</summary>
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>Gets or sets the confirmation password. Must match NewPassword.</summary>
        [Required(ErrorMessage = "Confirmation password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
