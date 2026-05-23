using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a search query executed by a user, logged for analytics.
    /// </summary>
    public class SearchLogDto
    {
        /// <summary>Gets or sets the unique identifier of the search log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the optional ID of the profile that performed the search.</summary>
        public long? ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the raw search string entered by the user.</summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>Gets or sets the total number of search results returned.</summary>
        public int? ResultsCount { get; set; }

        /// <summary>Gets or sets the optional ID of the show the user clicked on from the results.</summary>
        public long? ClickedShowId { get; set; }

        /// <summary>Gets or sets the title of the show the user clicked on.</summary>
        public string? ClickedShowTitle { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the search was performed.</summary>
        public DateTime? SearchAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a search log entry.
    /// </summary>
    public class SearchLogListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the search log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the raw search string.</summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>Gets or sets the total number of search results returned.</summary>
        public int? ResultsCount { get; set; }

        /// <summary>Gets or sets the title of the show clicked on.</summary>
        public string? ClickedShowTitle { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the search was performed.</summary>
        public DateTime? SearchAt { get; set; }
    }

    /// <summary>
    /// Request body for logging a new search query from the client.
    /// </summary>
    public class CreateSearchLogDto
    {
        /// <summary>Gets or sets the raw search string. Maximum 255 characters.</summary>
        [Required(ErrorMessage = "Query is required.")]
        [MaxLength(255, ErrorMessage = "Query cannot exceed 255 characters.")]
        public string Query { get; set; } = string.Empty;

        /// <summary>Gets or sets the total number of search results returned.</summary>
        public int? ResultsCount { get; set; }

        /// <summary>Gets or sets the optional ID of the show clicked on.</summary>
        public long? ClickedShowId { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for search logs shown in the admin dashboard.
    /// </summary>
    public class AdminSearchLogListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the search log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the raw search string.</summary>
        public string? Query { get; set; }

        /// <summary>Gets or sets the total number of search results returned.</summary>
        public int? ResultsCount { get; set; }

        /// <summary>Gets or sets the title of the show clicked on.</summary>
        public string? ClickedShowTitle { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the search was performed.</summary>
        public DateTime? SearchAt { get; set; }
        public long? ProfileId { get; set; }
        public long? ClickedShowId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

