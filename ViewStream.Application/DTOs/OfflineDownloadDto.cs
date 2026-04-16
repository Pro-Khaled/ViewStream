using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class OfflineDownloadDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public long DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? DownloadQuality { get; set; }
        public DateTime? DownloadedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? FilePath { get; set; }
    }

    public class OfflineDownloadListItemDto
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public string? DownloadQuality { get; set; }
        public DateTime? DownloadedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class CreateOfflineDownloadDto
    {
        public long EpisodeId { get; set; }
        public long DeviceId { get; set; }
        public string? DownloadQuality { get; set; }
        public string? FilePath { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    
}
