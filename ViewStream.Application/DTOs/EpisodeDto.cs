using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class EpisodeDto
    {
        public long Id { get; set; }
        public long SeasonId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? SeasonTitle { get; set; }
        public short SeasonNumber { get; set; }
        public short EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? RuntimeSeconds { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


    }

    public class EpisodeListItemDto
    {
        public long Id { get; set; }
        public short EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? RuntimeSeconds { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
    }

    public class CreateEpisodeDto
    {
        public long SeasonId { get; set; }
        public short EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? RuntimeSeconds { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }

        // Video file (uploaded separately)
        public IFormFile? VideoFile { get; set; }
    }

    public class UpdateEpisodeDto
    {
        public short EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? RuntimeSeconds { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
    }


}
