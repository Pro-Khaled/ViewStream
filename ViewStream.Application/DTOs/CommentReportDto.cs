using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class CommentReportDto
    {
        public long Id { get; set; }
        public long CommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string CommentAuthorName { get; set; } = string.Empty;
        public long ReportedByProfileId { get; set; }
        public string ReportedByProfileName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? Status { get; set; }
        public long? ReviewedByUserId { get; set; }
        public string? ReviewedByUserName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CommentReportListItemDto
    {
        public long Id { get; set; }
        public long CommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string ReportedByProfileName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateCommentReportDto
    {
        public long CommentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class UpdateReportStatusDto
    {
        public string Status { get; set; } = string.Empty; // "reviewed", "dismissed", "action_taken"
    }

}
