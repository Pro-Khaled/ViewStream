using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class LoginSessionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string SessionToken { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoginSessionListItemDto
    {
        public long Id { get; set; }
        public string? DeviceName { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

}
