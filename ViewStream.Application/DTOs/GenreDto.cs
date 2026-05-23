using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a genre used to categorize shows.
    /// </summary>
    public class GenreDto
    {
        /// <summary>Gets or sets the unique identifier of the genre.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the genre (e.g. "Action", "Comedy").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional group or category this genre belongs to.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the number of shows associated with this genre.</summary>
        public int ShowCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a genre.
    /// </summary>
    public class GenreListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the genre.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the genre.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of shows associated with this genre.</summary>
        public int ShowCount { get; set; }
    }

    /// <summary>
    /// Request body for creating a new genre.
    /// </summary>
    public class CreateGenreDto
    {
        /// <summary>Gets or sets the name of the genre. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for updating an existing genre.
    /// </summary>
    public class UpdateGenreDto
    {
        /// <summary>Gets or sets the updated name of the genre. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Admin list-item DTO for genres shown in the admin dashboard.
    /// </summary>
    public class AdminGenreListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the genre.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the genre.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of shows associated with this genre.</summary>
        public int ShowCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the genre was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets whether the genre is soft-deleted.</summary>
        public bool? IsDeleted { get; set; }
    }
}
