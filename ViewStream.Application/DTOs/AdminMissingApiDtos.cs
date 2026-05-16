using System;
using System.Collections.Generic;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Admin list item DTO for ItemVectors paging.
    /// </summary>
    public class AdminItemVectorListItemDto
    {
        /// <summary>
        /// Gets or sets the related show id.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the related show title.
        /// </summary>
        public string? ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets the last updated timestamp.
        /// </summary>
        public DateTime? LastUpdated { get; set; }
    }
}
