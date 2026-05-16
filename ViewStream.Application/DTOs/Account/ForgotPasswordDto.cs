using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    /// <summary>
    /// Request body for initiating a password reset process.
    /// </summary>
    public class ForgotPasswordDto
    {
        /// <summary>Gets or sets the user's email address to send the reset link to.</summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}
