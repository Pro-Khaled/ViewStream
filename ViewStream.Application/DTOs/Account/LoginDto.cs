using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    /// <summary>
    /// Request body for authenticating a user.
    /// </summary>
    public class LoginDto
    {
        /// <summary>Gets or sets the user's email address.</summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's password.</summary>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
