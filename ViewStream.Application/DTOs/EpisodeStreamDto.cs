namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Response DTO for episode streaming, includes quality-filtered stream URLs.
    /// </summary>
    public class EpisodeStreamDto
    {
        public long EpisodeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? HlsMasterUrl { get; set; }
        public int? DurationSeconds { get; set; }

        /// <summary>
        /// The qualities available to this user based on their subscription tier.
        /// e.g., ["360p", "480p", "720p"] for Basic, ["360p", "480p", "720p", "1080p", "4K"] for Premium.
        /// </summary>
        public List<string> AllowedQualities { get; set; } = new();

        /// <summary>
        /// Stream URLs filtered by the user's allowed quality tier.
        /// </summary>
        public List<StreamUrlDto> StreamUrls { get; set; } = new();
    }

    public class StreamUrlDto
    {
        public string Quality { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int? BitrateKbps { get; set; }
    }
}
