using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class AuditLogDto
    {
        public long Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public long RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public long? ChangedByUserId { get; set; }
        public string? ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Notes { get; set; }
    }

    public class AuditLogListItemDto
    {
        public long Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public long RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
    public class CreateAuditLogDto
    {
        public string TableName { get; set; } = string.Empty;
        public long RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public long? ChangedByUserId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Notes { get; set; }
    }

}
