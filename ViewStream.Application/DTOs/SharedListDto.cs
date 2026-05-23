using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a playlist or list of shows shared by a profile.
    /// </summary>
    public class SharedListDto
    {
        /// <summary>Gets or sets the unique identifier of the shared list.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that created this list.</summary>
        public long OwnerProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that created this list.</summary>
        public string OwnerProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the list.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional description of the list.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this list is publicly accessible.</summary>
        public bool? IsPublic { get; set; }

        /// <summary>Gets or sets the unique alphanumeric share code for direct linking.</summary>
        public string? ShareCode { get; set; }

        /// <summary>Gets or sets the number of items (shows/episodes) in this list.</summary>
        public int ItemCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the list was created.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for shared lists displayed in grids or public collections.
    /// </summary>
    public class SharedListListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the shared list.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the title of the list.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional description of the list.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the display name of the profile that created this list.</summary>
        public string OwnerProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of items in this list.</summary>
        public int ItemCount { get; set; }

        /// <summary>Gets or sets a value indicating whether this list is publicly accessible.</summary>
        public bool? IsPublic { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the list was created.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for creating a new shared list.
    /// </summary>
    public class CreateSharedListDto
    {
        /// <summary>Gets or sets the title of the list. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional description. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this list should be publicly accessible.</summary>
        public bool? IsPublic { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing shared list.
    /// </summary>
    public class UpdateSharedListDto
    {
        /// <summary>Gets or sets the title of the list. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional description. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        /// <summary>Gets or sets a value indicating whether this list should be publicly accessible.</summary>
        public bool? IsPublic { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for shared lists shown in the admin dashboard.
    /// </summary>
    public class AdminSharedListListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the shared list.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that owns this list.</summary>
        public long OwnerProfileId { get; set; }

        /// <summary>Gets or sets a value indicating whether this list is publicly accessible.</summary>
        public bool? IsPublic { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the list was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this list has been soft-deleted.</summary>
        public bool? IsDeleted { get; set; }

        /// <summary>Gets or sets the display name of the profile that owns this list.</summary>
        public string? OwnerProfileName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ShareCode { get; set; }
        public int ItemCount { get; set; }
    }
}

