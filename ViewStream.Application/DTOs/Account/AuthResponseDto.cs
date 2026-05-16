using System;
using System.Collections.Generic;

namespace ViewStream.Application.DTOs.Account
{
    /// <summary>
    /// Response returned upon successful authentication containing access and refresh tokens.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>Gets or sets the JWT access token for API authorization.</summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the opaque refresh token used to obtain a new access token.</summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when the access token expires.</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Gets or sets the authenticated user's profile summary.</summary>
        public UserDto User { get; set; } = new();
    }

    /// <summary>
    /// Slim user representation returned inside an authentication response.
    /// </summary>
    public class UserDto
    {
        /// <summary>Gets or sets the unique identifier of the user.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the user's email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's full name.</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's phone number.</summary>
        public string? Phone { get; set; }

        /// <summary>Gets or sets the user's country code.</summary>
        public string? CountryCode { get; set; }

        /// <summary>Gets or sets the list of role names assigned to the user.</summary>
        public List<string> Roles { get; set; } = new();
    }
}
