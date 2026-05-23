using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a person (e.g. actor, director, writer) involved in a show or episode.
    /// </summary>
    public class PersonDto
    {
        /// <summary>Gets or sets the unique identifier of the person.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the full name of the person.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the person's date of birth.</summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>Gets or sets a biographical description of the person.</summary>
        public string? Bio { get; set; }

        /// <summary>Gets or sets the URL to the person's photo.</summary>
        public string? PhotoUrl { get; set; }

        /// <summary>Gets or sets the total number of credits associated with this person.</summary>
        public int CreditCount { get; set; }

        /// <summary>Gets or sets the total number of awards won by this person.</summary>
        public int AwardCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a person.
    /// </summary>
    public class PersonListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the person.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the full name of the person.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the URL to the person's photo.</summary>
        public string? PhotoUrl { get; set; }

        /// <summary>Gets or sets the total number of credits associated with this person.</summary>
        public int CreditCount { get; set; }
    }

    /// <summary>
    /// Request body for creating a new person.
    /// </summary>
    public class CreatePersonDto
    {
        /// <summary>Gets or sets the full name of the person. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the date of birth.</summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>Gets or sets the biographical description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Bio cannot exceed 2000 characters.")]
        public string? Bio { get; set; }

        /// <summary>Gets or sets the URL to the person's photo. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "PhotoUrl cannot exceed 500 characters.")]
        public string? PhotoUrl { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing person.
    /// </summary>
    public class UpdatePersonDto
    {
        /// <summary>Gets or sets the updated full name. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the updated date of birth.</summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>Gets or sets the updated biographical description. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Bio cannot exceed 2000 characters.")]
        public string? Bio { get; set; }

        /// <summary>Gets or sets the updated URL to the photo. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "PhotoUrl cannot exceed 500 characters.")]
        public string? PhotoUrl { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for persons shown in the admin dashboard.
    /// </summary>
    public class AdminPersonListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the person.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the full name of the person.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the biographical description.</summary>
        public string? Bio { get; set; }

        /// <summary>Gets or sets the URL to the person's photo.</summary>
        public string? PhotoUrl { get; set; }

        /// <summary>Gets or sets the person's date of birth.</summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>Gets or sets the total number of credits.</summary>
        public int CreditCount { get; set; }

        /// <summary>Gets or sets the total number of awards.</summary>
        public int AwardCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this record was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets whether the person is soft-deleted.</summary>
        public bool? IsDeleted { get; set; }
    }
}
