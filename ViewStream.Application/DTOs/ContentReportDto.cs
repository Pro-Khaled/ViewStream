using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ContentReportDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public long? ShowId { get; set; }
        public string? ShowTitle { get; set; }
        public long? EpisodeId { get; set; }
        public string? EpisodeTitle { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class ContentReportListItemDto
    {
        public long Id { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty; // "Show" or "Episode"
        public string TargetTitle { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? ReportedAt { get; set; }
    }

    public class CreateContentReportDto
    {
        public long? ShowId { get; set; }
        public long? EpisodeId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateContentReportStatusDto
    {
        public string Status { get; set; } = string.Empty; // "pending", "reviewed", "dismissed", "action_taken"
    }


}
