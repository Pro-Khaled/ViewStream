namespace ViewStream.Application.DTOs
{
    /// <summary>DTO for trending show analytics.</summary>
    public class TrendingShowDto
    {
        public long ShowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public int ViewCount { get; set; }
        public decimal TrendScore { get; set; }
    }

    /// <summary>DTO for cohort retention analytics.</summary>
    public class CohortRetentionDto
    {
        public string CohortMonth { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public List<RetentionDataPoint> RetentionData { get; set; } = new();
    }

    public class RetentionDataPoint
    {
        public int MonthOffset { get; set; }
        public int ActiveUsers { get; set; }
        public decimal RetentionRate { get; set; }
    }

    /// <summary>DTO for DAU/MAU engagement metrics.</summary>
    public class DauMauDto
    {
        public DateOnly Date { get; set; }
        public int DailyActiveUsers { get; set; }
        public int MonthlyActiveUsers { get; set; }
        public decimal DauMauRatio { get; set; }
    }
}
