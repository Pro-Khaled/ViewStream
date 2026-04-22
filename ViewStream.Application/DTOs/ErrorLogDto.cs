using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ErrorLogDto
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? Endpoint { get; set; }
        public DateTime? OccurredAt { get; set; }
    }

    public class ErrorLogListItemDto
    {
        public long Id { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Endpoint { get; set; }
        public DateTime? OccurredAt { get; set; }
    }

    public class CreateErrorLogDto
    {
        public long? UserId { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? Endpoint { get; set; }
    }

}
