using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    /// <summary>
    /// Request body for obtaining a new access token using an existing refresh token.
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>Gets or sets the opaque refresh token.</summary>
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
