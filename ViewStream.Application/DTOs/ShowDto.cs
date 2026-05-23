using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a single show, including genres, tags, and content statistics.
    /// </summary>
    public class ShowDto
    {
        /// <summary>Gets or sets the unique identifier of the show.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional synopsis/description of the show.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the year the show was first released.</summary>
        public short? ReleaseYear { get; set; }

        /// <summary>Gets or sets the maturity/age rating of the show (e.g. "PG-13", "R").</summary>
        public string? MaturityRating { get; set; }

        /// <summary>Gets or sets the average runtime per episode in minutes.</summary>
        public short? RuntimeMinutes { get; set; }

        /// <summary>Gets or sets the public URL of the show poster image.</summary>
        public string? PosterUrl { get; set; }

        /// <summary>Gets or sets the public URL of the show backdrop image.</summary>
        public string? BackdropUrl { get; set; }

        /// <summary>Gets or sets the public URL of the show trailer video.</summary>
        public string? TrailerUrl { get; set; }

        /// <summary>Gets or sets the IMDb rating (0.0 – 10.0).</summary>
        public decimal? ImdbRating { get; set; }

        /// <summary>Gets or sets the Rotten Tomatoes score (0 – 100).</summary>
        public short? RottenTomatoesScore { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this show was added to the catalogue.</summary>
        public DateTime? AddedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this show was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this show has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this show was soft-deleted, if applicable.</summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>Gets or sets the list of genre names associated with this show.</summary>
        public List<string> Genres { get; set; } = new();

        /// <summary>Gets or sets the list of tag names associated with this show.</summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>Gets or sets the number of seasons available for this show.</summary>
        public int SeasonCount { get; set; }

        /// <summary>Gets or sets the total number of episodes across all seasons.</summary>
        public int EpisodeCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO used in public show browse listings.
    /// </summary>
    public class ShowListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the show.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the year the show was first released.</summary>
        public short? ReleaseYear { get; set; }

        /// <summary>Gets or sets the maturity/age rating of the show.</summary>
        public string? MaturityRating { get; set; }

        /// <summary>Gets or sets the URL of the show poster image.</summary>
        public string? PosterUrl { get; set; }

        /// <summary>Gets or sets the IMDb rating (0.0 – 10.0).</summary>
        public decimal? ImdbRating { get; set; }

        /// <summary>Gets or sets the list of genre names associated with this show.</summary>
        public List<string> Genres { get; set; } = new();
    }

    /// <summary>
    /// Request body for creating a new show.
    /// </summary>
    public class CreateShowDto
    {
        /// <summary>Gets or sets the title of the show. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional synopsis/description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the year the show was first released (1888 – 2100).</summary>
        [Range(1888, 2100, ErrorMessage = "ReleaseYear must be between 1888 and 2100.")]
        public short? ReleaseYear { get; set; }

        /// <summary>Gets or sets the maturity/age rating (e.g. "PG-13"). Maximum 10 characters.</summary>
        [MaxLength(10, ErrorMessage = "MaturityRating cannot exceed 10 characters.")]
        public string? MaturityRating { get; set; }

        /// <summary>Gets or sets the average runtime per episode in minutes.</summary>
        [Range(1, short.MaxValue, ErrorMessage = "RuntimeMinutes must be positive.")]
        public short? RuntimeMinutes { get; set; }

        /// <summary>Gets or sets the public URL of the show poster image. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "PosterUrl cannot exceed 500 characters.")]
        public string? PosterUrl { get; set; }

        /// <summary>Gets or sets the public URL of the show backdrop image. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "BackdropUrl cannot exceed 500 characters.")]
        public string? BackdropUrl { get; set; }

        /// <summary>Gets or sets the public URL of the show trailer. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "TrailerUrl cannot exceed 500 characters.")]
        public string? TrailerUrl { get; set; }

        /// <summary>Gets or sets the IMDb rating (0.0 – 10.0).</summary>
        [Range(0.0, 10.0, ErrorMessage = "ImdbRating must be between 0 and 10.")]
        public decimal? ImdbRating { get; set; }

        /// <summary>Gets or sets the Rotten Tomatoes score (0 – 100).</summary>
        [Range(0, 100, ErrorMessage = "RottenTomatoesScore must be between 0 and 100.")]
        public short? RottenTomatoesScore { get; set; }

        /// <summary>Gets or sets the IDs of genres to associate with this show.</summary>
        public List<long> GenreIds { get; set; } = new();

        /// <summary>Gets or sets the IDs of tags to associate with this show.</summary>
        public List<long> TagIds { get; set; } = new();
    }

    /// <summary>
    /// Request body for updating an existing show.
    /// </summary>
    public class UpdateShowDto
    {
        /// <summary>Gets or sets the title of the show. Maximum 500 characters.</summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional synopsis/description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets the year the show was first released (1888 – 2100).</summary>
        [Range(1888, 2100, ErrorMessage = "ReleaseYear must be between 1888 and 2100.")]
        public short? ReleaseYear { get; set; }

        /// <summary>Gets or sets the maturity/age rating. Maximum 10 characters.</summary>
        [MaxLength(10, ErrorMessage = "MaturityRating cannot exceed 10 characters.")]
        public string? MaturityRating { get; set; }

        /// <summary>Gets or sets the average runtime per episode in minutes.</summary>
        [Range(1, short.MaxValue, ErrorMessage = "RuntimeMinutes must be positive.")]
        public short? RuntimeMinutes { get; set; }

        /// <summary>Gets or sets the public URL of the show poster image. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "PosterUrl cannot exceed 500 characters.")]
        public string? PosterUrl { get; set; }

        /// <summary>Gets or sets the public URL of the show backdrop image. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "BackdropUrl cannot exceed 500 characters.")]
        public string? BackdropUrl { get; set; }

        /// <summary>Gets or sets the public URL of the show trailer. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "TrailerUrl cannot exceed 500 characters.")]
        public string? TrailerUrl { get; set; }

        /// <summary>Gets or sets the IMDb rating (0.0 – 10.0).</summary>
        [Range(0.0, 10.0, ErrorMessage = "ImdbRating must be between 0 and 10.")]
        public decimal? ImdbRating { get; set; }

        /// <summary>Gets or sets the Rotten Tomatoes score (0 – 100).</summary>
        [Range(0, 100, ErrorMessage = "RottenTomatoesScore must be between 0 and 100.")]
        public short? RottenTomatoesScore { get; set; }

        /// <summary>Gets or sets the IDs of genres to associate with this show.</summary>
        public List<long> GenreIds { get; set; } = new();

        /// <summary>Gets or sets the IDs of tags to associate with this show.</summary>
        public List<long> TagIds { get; set; } = new();
    }

    /// <summary>
    /// Admin list-item DTO for shows shown in the admin dashboard.
    /// </summary>
    public class AdminShowListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the show.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the year the show was first released.</summary>
        public short? ReleaseYear { get; set; }

        /// <summary>Gets or sets the maturity/age rating of the show.</summary>
        public string? MaturityRating { get; set; }

        /// <summary>Gets or sets the URL of the show poster image.</summary>
        public string? PosterUrl { get; set; }

        /// <summary>Gets or sets the IMDb rating (0.0 – 10.0).</summary>
        public decimal? ImdbRating { get; set; }

        /// <summary>Gets or sets the list of genre names associated with this show.</summary>
        public List<string> Genres { get; set; } = new();

        /// <summary>Gets or sets a value indicating whether this show has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this show was added to the catalogue.</summary>
        public DateTime? AddedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this show was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets the Rotten Tomatoes score (0 – 100).</summary>
        public short? RottenTomatoesScore { get; set; }

        /// <summary>Gets or sets the number of seasons available for this show.</summary>
        public int SeasonCount { get; set; }

        /// <summary>Gets or sets the total number of episodes across all seasons.</summary>
        public int EpisodeCount { get; set; }
        public string? Description { get; set; }
        public string? BackdropUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

