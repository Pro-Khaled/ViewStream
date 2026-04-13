using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class WatchHistoryDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public long SeasonId { get; set; }
        public short SeasonNumber { get; set; }
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public DateTime? WatchedAt { get; set; }
        public int? ProgressSeconds { get; set; }
        public int? TotalSeconds { get; set; }
        public bool? Completed { get; set; }
    }

    public class WatchHistoryListItemDto
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public int? ProgressSeconds { get; set; }
        public int? TotalSeconds { get; set; }
        public bool? Completed { get; set; }
        public DateTime? WatchedAt { get; set; }
    }

    public class CreateUpdateWatchHistoryDto
    {
        public long EpisodeId { get; set; }
        public int? ProgressSeconds { get; set; }
        public bool? Completed { get; set; }
    }
}
