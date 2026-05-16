using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an individual item (show) within a shared list.
    /// </summary>
    public class SharedListItemDto
    {
        /// <summary>Gets or sets the ID of the shared list.</summary>
        public long ListId { get; set; }

        /// <summary>Gets or sets the title of the shared list.</summary>
        public string ListName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the show added to the list.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show added to the list.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the poster URL of the show.</summary>
        public string? ShowPosterUrl { get; set; }

        /// <summary>Gets or sets the release year of the show.</summary>
        public short? ReleaseYear { get; set; }

        /// <summary>Gets or sets the ID of the profile that added this item.</summary>
        public long? AddedByProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile that added this item.</summary>
        public string? AddedByProfileName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the item was added.</summary>
        public DateTime? AddedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for rendering items inside a shared list view.
    /// </summary>
    public class SharedListItemListItemDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the poster URL of the show.</summary>
        public string? ShowPosterUrl { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the item was added.</summary>
        public DateTime? AddedAt { get; set; }
    }

    /// <summary>
    /// Request body for adding a new show to a shared list.
    /// </summary>
    public class AddShowToSharedListDto
    {
        /// <summary>Gets or sets the ID of the show to add.</summary>
        [Required(ErrorMessage = "ShowId is required.")]
        public long ShowId { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for shared list items shown in the admin dashboard.
    /// </summary>
    public class AdminSharedListItemListItemDto
    {
        /// <summary>Gets or sets the ID of the shared list.</summary>
        public long ListId { get; set; }

        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the display name of the profile that added this item.</summary>
        public string? AddedByProfileName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the item was added.</summary>
        public DateTime? AddedAt { get; set; }
    }
}
