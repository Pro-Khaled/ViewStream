using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        // Required settings
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        // Token expiration settings
        public int ExpiryMinutes { get; set; } = 60;
        public int RefreshTokenExpiryDays { get; set; } = 7;

        // Advanced settings
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;
        public bool ValidateLifetime { get; set; } = true;
        public bool ValidateIssuerSigningKey { get; set; } = true;
        public int ClockSkewSeconds { get; set; } = 0;

        // Algorithm settings
        public string Algorithm { get; set; } = "HmacSha256";

        // Optional: For refresh tokens
        public int RefreshTokenLength { get; set; } = 32;
        public bool AllowMultipleDevices { get; set; } = false;

        // Optional: For claims
        public string NameClaimType { get; set; } = "name";
        public string RoleClaimType { get; set; } = "role";
    }
}
