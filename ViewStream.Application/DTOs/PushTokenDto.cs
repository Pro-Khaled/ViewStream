using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PushTokenDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? Platform { get; set; }
        public DateTime? LastUsed { get; set; }
    }

    public class CreatePushTokenDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? Platform { get; set; }
    }
}
