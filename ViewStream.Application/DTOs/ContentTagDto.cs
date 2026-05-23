using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a tag used to categorize shows (e.g. "Action", "Award-Winning", "Dark").
    /// </summary>
    public class ContentTagDto
    {
        /// <summary>Gets or sets the unique identifier of the tag.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the tag.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional group or category this tag belongs to.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the number of shows associated with this tag.</summary>
        public int ShowCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a content tag.
    /// </summary>
    public class ContentTagListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the tag.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the tag.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional group or category this tag belongs to.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the number of shows associated with this tag.</summary>
        public int ShowCount { get; set; }
    }

    /// <summary>
    /// Request body for creating a new content tag.
    /// </summary>
    public class CreateContentTagDto
    {
        /// <summary>Gets or sets the name of the tag. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional category. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string? Category { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing content tag.
    /// </summary>
    public class UpdateContentTagDto
    {
        /// <summary>Gets or sets the updated name of the tag. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the updated category. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string? Category { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for content tags shown in the admin dashboard.
    /// </summary>
    public class AdminContentTagListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the tag.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the name of the tag.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional group or category this tag belongs to.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets the number of shows associated with this tag.</summary>
        public int ShowCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the tag was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets whether the tag is soft-deleted.</summary>
        public bool IsDeleted { get; set; }
    }
}
