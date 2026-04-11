using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ShowDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public short? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
        public short? RuntimeMinutes { get; set; }
        public string? PosterUrl { get; set; }
        public string? BackdropUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public decimal? ImdbRating { get; set; }
        public short? RottenTomatoesScore { get; set; }
        public DateTime? AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Related data
        public List<string> Genres { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public int SeasonCount { get; set; }
        public int EpisodeCount { get; set; }
    }

    public class ShowListItemDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public short? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
        public string? PosterUrl { get; set; }
        public decimal? ImdbRating { get; set; }
        public List<string> Genres { get; set; } = new();
    }

    public class CreateShowDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public short? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
        public short? RuntimeMinutes { get; set; }
        public string? PosterUrl { get; set; }
        public string? BackdropUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public decimal? ImdbRating { get; set; }
        public short? RottenTomatoesScore { get; set; }

        public List<long> GenreIds { get; set; } = new();
        public List<long> TagIds { get; set; } = new();
    }
    public class UpdateShowDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public short? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
        public short? RuntimeMinutes { get; set; }
        public string? PosterUrl { get; set; }
        public string? BackdropUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public decimal? ImdbRating { get; set; }
        public short? RottenTomatoesScore { get; set; }

        public List<long> GenreIds { get; set; } = new();
        public List<long> TagIds { get; set; } = new();
    }
}
