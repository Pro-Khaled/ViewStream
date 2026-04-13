using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class SeasonDto
    {
        public long Id { get; set; }
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public short SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int EpisodeCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SeasonListItemDto
    {
        public long Id { get; set; }
        public short SeasonNumber { get; set; }
        public string? Title { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int EpisodeCount { get; set; }
    }

    public class CreateSeasonDto
    {
        public long ShowId { get; set; }
        public short SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
    }

    public class UpdateSeasonDto
    {
        public short SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
    }

}
