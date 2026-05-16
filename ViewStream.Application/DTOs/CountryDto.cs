using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a country entity used for content availability and licensing.
    /// </summary>
    public class CountryDto
    {
        /// <summary>Gets or sets the ISO country code.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the country.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the continent the country belongs to.</summary>
        public string? Continent { get; set; }

        /// <summary>Gets or sets the number of shows available in this country.</summary>
        public int AvailabilityCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a country.
    /// </summary>
    public class CountryListItemDto
    {
        /// <summary>Gets or sets the ISO country code.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the country.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the continent the country belongs to.</summary>
        public string? Continent { get; set; }
    }

    /// <summary>
    /// Request body for adding a new country.
    /// </summary>
    public class CreateCountryDto
    {
        /// <summary>Gets or sets the ISO country code. Exact 2 characters.</summary>
        [Required(ErrorMessage = "Code is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Code must be exactly 2 characters.")]
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the country. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the continent. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "Continent cannot exceed 50 characters.")]
        public string? Continent { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing country.
    /// </summary>
    public class UpdateCountryDto
    {
        /// <summary>Gets or sets the updated name. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the updated continent. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "Continent cannot exceed 50 characters.")]
        public string? Continent { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for countries shown in the admin dashboard.
    /// </summary>
    public class AdminCountryListItemDto
    {
        /// <summary>Gets or sets the ISO country code.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the country.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the continent the country belongs to.</summary>
        public string? Continent { get; set; }

        /// <summary>Gets or sets the number of shows available in this country.</summary>
        public int AvailabilityCount { get; set; }
    }
}
