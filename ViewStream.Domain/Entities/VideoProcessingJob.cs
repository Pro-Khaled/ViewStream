using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Domain.Entities
{
    public class VideoProcessingJob
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public string SourceFileUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";   // Pending, Processing, Completed, Failed
        public string? HlsMasterUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Episode Episode { get; set; } = null!;
    }
}
