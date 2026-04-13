using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class AudioTrackDto
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string? TrackType { get; set; }
        public string AudioUrl { get; set; } = string.Empty;
        public bool? IsDefault { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AudioTrackListItemDto
    {
        public long Id { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string? TrackType { get; set; }
        public bool? IsDefault { get; set; }
    }
    public class CreateAudioTrackDto
    {
        public long EpisodeId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string? TrackType { get; set; }
        public string AudioUrl { get; set; } = string.Empty;
        public bool? IsDefault { get; set; }
    }

    public class UpdateAudioTrackDto    
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string? TrackType { get; set; }
        public string AudioUrl { get; set; } = string.Empty;
        public bool? IsDefault { get; set; }
    }

}
