using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a machine learning feature vector representing a user profile for the recommendation engine.
    /// </summary>
    public class UserVectorDto
    {
        /// <summary>Gets or sets the ID of the profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the serialized JSON array representing the high-dimensional user preference embedding.</summary>
        public string? EmbeddingJson { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the vector was last recalculated.</summary>
        public DateTime? LastUpdated { get; set; }
    }

    /// <summary>
    /// Request body for manually updating the machine learning vector of a user profile.
    /// </summary>
    public class CreateUpdateUserVectorDto
    {
        /// <summary>Gets or sets the serialized JSON array representing the feature embedding.</summary>
        [Required(ErrorMessage = "EmbeddingJson is required.")]
        public string? EmbeddingJson { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for user vectors shown in the admin dashboard.
    /// </summary>
    public class AdminUserVectorListItemDto
    {
        /// <summary>Gets or sets the ID of the profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the vector was last updated.</summary>
        public DateTime? LastUpdated { get; set; }
    }
}
