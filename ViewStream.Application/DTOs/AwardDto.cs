using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an award that can be associated with shows or persons.
    /// </summary>
    public class AwardDto
    {
        /// <summary>Gets or sets the unique identifier of the award.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the category of the award.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public short? Year { get; set; }

        /// <summary>Gets or sets the number of persons associated with this award.</summary>
        public int PersonAwardCount { get; set; }

        /// <summary>Gets or sets the number of shows associated with this award.</summary>
        public int ShowAwardCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for an award.
    /// </summary>
    public class AwardListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the award.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the category of the award.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public short? Year { get; set; }
    }

    /// <summary>
    /// Request body for creating a new award.
    /// </summary>
    public class CreateAwardDto
    {
        /// <summary>Gets or sets the name of the award. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the category. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string? Category { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public short? Year { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing award.
    /// </summary>
    public class UpdateAwardDto
    {
        /// <summary>Gets or sets the updated name of the award. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the updated category. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string? Category { get; set; }

        /// <summary>Gets or sets the updated year the award was given.</summary>
        public short? Year { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for awards shown in the admin dashboard.
    /// </summary>
    public class AdminAwardListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the award.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the award.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the category of the award.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the year the award was given.</summary>
        public short? Year { get; set; }

        /// <summary>Gets or sets the number of persons associated with this award.</summary>
        public int PersonAwardCount { get; set; }

        /// <summary>Gets or sets the number of shows associated with this award.</summary>
        public int ShowAwardCount { get; set; }
    }
}
