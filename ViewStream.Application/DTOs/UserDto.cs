using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockedReason { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class AdminUpdateUserDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public List<string>? Roles { get; set; }
    }

    public class BlockUserDto
    {
        public string Reason { get; set; } = string.Empty;
        public DateTime? BlockedUntil { get; set; }
    }
}
