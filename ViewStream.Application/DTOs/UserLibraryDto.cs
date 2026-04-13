using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class UserLibraryDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public long? ShowId { get; set; }
        public string? ShowTitle { get; set; }
        public string? ShowPosterUrl { get; set; }
        public long? SeasonId { get; set; }
        public string? SeasonTitle { get; set; }
        public short? SeasonNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? EpisodesWatched { get; set; }
        public decimal? UserScore { get; set; }
        public DateOnly? StartedAt { get; set; }
        public DateOnly? CompletedAt { get; set; }
        public DateTime? AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserLibraryListItemDto
    {
        public long Id { get; set; }
        public long? ShowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public string ItemType { get; set; } = "Show"; // "Show" or "Season"
        public string Status { get; set; } = string.Empty;
        public int? EpisodesWatched { get; set; }
        public int? TotalEpisodes { get; set; }
        public decimal? UserScore { get; set; }
        public DateTime? AddedAt { get; set; }
    }

    public class CreateUserLibraryDto
    {
        public long? ShowId { get; set; }
        public long? SeasonId { get; set; }
        public string Status { get; set; } = "plan_to_watch";
        public int? EpisodesWatched { get; set; }
        public decimal? UserScore { get; set; }
        public DateOnly? StartedAt { get; set; }
        public DateOnly? CompletedAt { get; set; }
    }

    public class UpdateUserLibraryDto
    {
        public string? Status { get; set; }
        public int? EpisodesWatched { get; set; }
        public decimal? UserScore { get; set; }
        public DateOnly? StartedAt { get; set; }
        public DateOnly? CompletedAt { get; set; }
    }

    public class UserLibrarySummaryDto
    {
        public long ProfileId { get; set; }
        public int TotalItems { get; set; }
        public Dictionary<string, int> CountByStatus { get; set; } = new();
    }


}
