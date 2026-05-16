using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a dynamically generated personalized recommendation row for a profile.
    /// </summary>
    public class PersonalizedRowDto
    {
        /// <summary>Gets or sets the ID of the profile this row was generated for.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the row (e.g. "Because you watched Inception").</summary>
        public string RowName { get; set; } = string.Empty;

        /// <summary>Gets or sets the list of show IDs recommended in this row.</summary>
        public List<long> ShowIds { get; set; } = new();

        /// <summary>Gets or sets the UTC timestamp when the row was generated.</summary>
        public DateTime? GeneratedAt { get; set; }
    }

    /// <summary>
    /// Request body for manually defining or updating a personalized row.
    /// </summary>
    public class CreateUpdatePersonalizedRowDto
    {
        /// <summary>Gets or sets the display name of the row. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "RowName is required.")]
        [MaxLength(100, ErrorMessage = "RowName cannot exceed 100 characters.")]
        public string RowName { get; set; } = string.Empty;

        /// <summary>Gets or sets the list of recommended show IDs.</summary>
        [Required(ErrorMessage = "ShowIds list cannot be null.")]
        public List<long> ShowIds { get; set; } = new();
    }

    /// <summary>
    /// Admin list-item DTO for personalized rows shown in the admin dashboard.
    /// </summary>
    public class AdminPersonalizedRowListItemDto
    {
        /// <summary>Gets or sets the ID of the profile this row belongs to.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the row.</summary>
        public string? RowName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this row was last generated.</summary>
        public DateTime? GeneratedAt { get; set; }

        /// <summary>Gets or sets the total number of show items contained in the row.</summary>
        public int ItemCount { get; set; }
    }
}
