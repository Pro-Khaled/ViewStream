using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an award won or nominated for by a show.
    /// </summary>
    public class ShowAwardDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the award.</summary>
        public int AwardId { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string AwardName { get; set; } = string.Empty;

        /// <summary>Gets or sets the category of the award.</summary>
        public string? AwardCategory { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public short? AwardYear { get; set; }

        /// <summary>Gets or sets a value indicating whether the show won the award.</summary>
        public bool? Won { get; set; }
    }

    /// <summary>
    /// Request body for associating a show with an award.
    /// </summary>
    public class CreateShowAwardDto
    {
        /// <summary>Gets or sets the ID of the award.</summary>
        [Required(ErrorMessage = "AwardId is required.")]
        public int AwardId { get; set; }

        /// <summary>Gets or sets a value indicating whether the show won the award.</summary>
        public bool? Won { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for show awards shown in the admin dashboard.
    /// </summary>
    public class AdminShowAwardListItemDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the ID of the award.</summary>
        public long AwardId { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string? AwardName { get; set; }

        /// <summary>Gets or sets a value indicating whether the show won the award.</summary>
        public bool? Won { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public int? AwardYear { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? AwardCategory { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

