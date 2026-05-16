using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a rating submitted by a user for a show.
    /// </summary>
    public class RatingDto
    {
        /// <summary>Gets or sets the ID of the profile that rated the show.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the show that was rated.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the rating score assigned (e.g. 1 to 5).</summary>
        public short Rating { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the rating was submitted or last updated.</summary>
        public DateTime? RatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for viewing individual user ratings on a show.
    /// </summary>
    public class RatingListItemDto
    {
        /// <summary>Gets or sets the ID of the profile that rated the show.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the rating score assigned.</summary>
        public short Rating { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the rating was submitted.</summary>
        public DateTime? RatedAt { get; set; }
    }

    /// <summary>
    /// Request body for creating or updating a show rating.
    /// </summary>
    public class CreateUpdateRatingDto
    {
        /// <summary>Gets or sets the ID of the show to rate.</summary>
        [Required(ErrorMessage = "ShowId is required.")]
        public long ShowId { get; set; }

        /// <summary>Gets or sets the rating score. Must be between 1 and 5.</summary>
        [Required(ErrorMessage = "Rating score is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public short Rating { get; set; }
    }

    /// <summary>
    /// Aggregate summary DTO of ratings for a specific show.
    /// </summary>
    public class ShowRatingSummaryDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the calculated average rating score.</summary>
        public double AverageRating { get; set; }

        /// <summary>Gets or sets the total number of ratings submitted.</summary>
        public int TotalRatings { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for raw ratings shown in the admin dashboard.
    /// </summary>
    public class AdminRatingListItemDto
    {
        /// <summary>Gets or sets the ID of the profile that rated the show.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the ID of the show that was rated.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the rating score assigned.</summary>
        public int? Score { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the rating was submitted.</summary>
        public DateTime? RatedAt { get; set; }
    }
}
