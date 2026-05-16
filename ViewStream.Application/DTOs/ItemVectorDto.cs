using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a machine learning feature vector representing a show for the recommendation engine.
    /// </summary>
    public class ItemVectorDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the serialized JSON array representing the high-dimensional feature embedding.</summary>
        public string? EmbeddingJson { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the vector was last recalculated.</summary>
        public DateTime? LastUpdated { get; set; }
    }

    /// <summary>
    /// Request body for manually updating the machine learning vector of a show.
    /// </summary>
    public class CreateUpdateItemVectorDto
    {
        /// <summary>Gets or sets the serialized JSON array representing the feature embedding.</summary>
        [Required(ErrorMessage = "EmbeddingJson is required.")]
        public string? EmbeddingJson { get; set; }
    }
}
