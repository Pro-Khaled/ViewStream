using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a show's availability in a specific country.
    /// </summary>
    public class ShowAvailabilityDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the ISO country code.</summary>
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the country.</summary>
        public string CountryName { get; set; } = string.Empty;

        /// <summary>Gets or sets the date the show becomes available in this country.</summary>
        public DateOnly? AvailableFrom { get; set; }

        /// <summary>Gets or sets the date the show's licensing expires in this country.</summary>
        public DateOnly? AvailableUntil { get; set; }

        /// <summary>Gets or sets internal administrative notes regarding the licensing agreement.</summary>
        public string? LicensingNotes { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for show availability.
    /// </summary>
    public class ShowAvailabilityListItemDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the ISO country code.</summary>
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the country.</summary>
        public string CountryName { get; set; } = string.Empty;

        /// <summary>Gets or sets the date the show becomes available.</summary>
        public DateOnly? AvailableFrom { get; set; }

        /// <summary>Gets or sets the date the show's licensing expires.</summary>
        public DateOnly? AvailableUntil { get; set; }
    }

    /// <summary>
    /// Request body for adding availability for a show in a new country.
    /// </summary>
    public class CreateShowAvailabilityDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        [Required(ErrorMessage = "ShowId is required.")]
        public long ShowId { get; set; }

        /// <summary>Gets or sets the ISO country code. Exact 2 characters.</summary>
        [Required(ErrorMessage = "CountryCode is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "CountryCode must be exactly 2 characters.")]
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the date the show becomes available.</summary>
        public DateOnly? AvailableFrom { get; set; }

        /// <summary>Gets or sets the date the licensing expires.</summary>
        public DateOnly? AvailableUntil { get; set; }

        /// <summary>Gets or sets optional licensing notes. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "LicensingNotes cannot exceed 500 characters.")]
        public string? LicensingNotes { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing availability record.
    /// </summary>
    public class UpdateShowAvailabilityDto
    {
        /// <summary>Gets or sets the updated availability start date.</summary>
        public DateOnly? AvailableFrom { get; set; }

        /// <summary>Gets or sets the updated availability end date.</summary>
        public DateOnly? AvailableUntil { get; set; }

        /// <summary>Gets or sets the updated licensing notes. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "LicensingNotes cannot exceed 500 characters.")]
        public string? LicensingNotes { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for show availability shown in the admin dashboard.
    /// </summary>
    public class AdminShowAvailabilityListItemDto
    {
        /// <summary>Gets or sets the ID of the show.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the ISO country code.</summary>
        public string? CountryCode { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the record was added.</summary>
        public DateTime? AddedAt { get; set; }
    }
}
