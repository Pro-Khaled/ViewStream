using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class DataDeletionRequestDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime? RequestedAt { get; set; }
        public string? Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ConfirmationCode { get; set; }
    }


    public class DataDeletionRequestListItemDto
    {
        public long Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? RequestedAt { get; set; }
    }

    public class UpdateDataDeletionRequestDto
    {
        public string? Status { get; set; }       // "completed", "processing"
        public string? ConfirmationCode { get; set; }
    }

}
