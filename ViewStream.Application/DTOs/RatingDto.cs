using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class RatingDto
    {
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public short Rating { get; set; }
        public DateTime? RatedAt { get; set; }
    }
    public class RatingListItemDto
    {
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public short Rating { get; set; }
        public DateTime? RatedAt { get; set; }
    }

    public class CreateUpdateRatingDto
    {
        public long ShowId { get; set; }
        public short Rating { get; set; }
    }
    public class ShowRatingSummaryDto
    {
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

}
