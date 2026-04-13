using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class SubtitleDto
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string SubtitleUrl { get; set; } = string.Empty;
        public bool? IsCc { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class SubtitleListItemDto
    {
        public long Id { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string SubtitleUrl { get; set; } = string.Empty;
        public bool? IsCc { get; set; }
    }

    public class CreateSubtitleDto
    {
        public long EpisodeId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string SubtitleUrl { get; set; } = string.Empty;
        public bool? IsCc { get; set; }
    }

    public class UpdateSubtitleDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string SubtitleUrl { get; set; } = string.Empty;
        public bool? IsCc { get; set; }
    }

}
