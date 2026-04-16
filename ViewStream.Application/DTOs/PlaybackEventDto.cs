using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PlaybackEventDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public long EpisodeId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? PositionSeconds { get; set; }
        public string? Quality { get; set; }
        public int? BitrateKbps { get; set; }
        public int? BufferingCount { get; set; }
        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreatePlaybackEventDto
    {
        public long EpisodeId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? PositionSeconds { get; set; }
        public string? Quality { get; set; }
        public int? BitrateKbps { get; set; }
        public int? BufferingCount { get; set; }
        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

}
