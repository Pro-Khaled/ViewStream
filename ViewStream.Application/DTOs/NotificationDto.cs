using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string? NotificationType { get; set; }
        public string? DataJson { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateNotificationDto
    {
        public long UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string? NotificationType { get; set; }
        public string? DataJson { get; set; }
    }


}
