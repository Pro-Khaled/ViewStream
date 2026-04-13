using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ProfileDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool? IsKids { get; set; }
        public string? AvatarIcon { get; set; }
        public string? LanguagePref { get; set; }
        public short? MaturityLevel { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProfileListItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsKids { get; set; }
        public string? AvatarIcon { get; set; }
    }

    public class CreateProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public bool? IsKids { get; set; }
        public string? AvatarIcon { get; set; }
        public string? LanguagePref { get; set; }
        public short? MaturityLevel { get; set; }
    }

    public class UpdateProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public bool? IsKids { get; set; }
        public string? AvatarIcon { get; set; }
        public string? LanguagePref { get; set; }
        public short? MaturityLevel { get; set; }
    }

    public class SwitchProfileResponseDto
    {
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty; // New JWT with updated ProfileId claim
    }

}
