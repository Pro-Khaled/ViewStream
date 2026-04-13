using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class UserInteractionDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string InteractionType { get; set; } = string.Empty;
        public decimal? Weight { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UserInteractionListItemDto
    {
        public long Id { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string ShowTitle { get; set; } = string.Empty;
        public string InteractionType { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateUserInteractionDto
    {
        public long ShowId { get; set; }
        public string InteractionType { get; set; } = string.Empty;
        public decimal? Weight { get; set; } = 1.0m;
    }

    public class UserInteractionSummaryDto
    {
        public long ProfileId { get; set; }
        public int TotalInteractions { get; set; }
        public Dictionary<string, int> InteractionsByType { get; set; } = new();
        public List<TopShowDto> TopShows { get; set; } = new();

        public class TopShowDto
        {
            public long ShowId { get; set; }
            public string ShowTitle { get; set; } = string.Empty;
            public int InteractionCount { get; set; }
        }
    }

}
