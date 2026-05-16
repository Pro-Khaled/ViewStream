using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an award associated with a specific person (actor/director/etc).
    /// </summary>
    public class PersonAwardDto
    {
        /// <summary>Gets or sets the ID of the person.</summary>
        public long PersonId { get; set; }

        /// <summary>Gets or sets the name of the person.</summary>
        public string PersonName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the award.</summary>
        public int AwardId { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string AwardName { get; set; } = string.Empty;

        /// <summary>Gets or sets the category of the award.</summary>
        public string? AwardCategory { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public short? AwardYear { get; set; }

        /// <summary>Gets or sets a value indicating whether the person won the award.</summary>
        public bool? Won { get; set; }
    }

    /// <summary>
    /// Request body for associating an award with a person.
    /// </summary>
    public class CreatePersonAwardDto
    {
        /// <summary>Gets or sets the ID of the award.</summary>
        [Required(ErrorMessage = "AwardId is required.")]
        public int AwardId { get; set; }

        /// <summary>Gets or sets a value indicating whether the person won the award.</summary>
        public bool? Won { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for person awards shown in the admin dashboard.
    /// </summary>
    public class AdminPersonAwardListItemDto
    {
        /// <summary>Gets or sets the ID of the person.</summary>
        public long PersonId { get; set; }

        /// <summary>Gets or sets the ID of the award.</summary>
        public long AwardId { get; set; }

        /// <summary>Gets or sets the name of the person.</summary>
        public string? PersonName { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string? AwardName { get; set; }

        /// <summary>Gets or sets a value indicating whether the person won the award.</summary>
        public bool? Won { get; set; }

        /// <summary>Gets or sets the category of the award.</summary>
        public string? AwardCategory { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public int? AwardYear { get; set; }
    }
}
