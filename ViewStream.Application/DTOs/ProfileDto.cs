using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a user profile.
    /// </summary>
    public class ProfileDto
    {
        /// <summary>Gets or sets the unique identifier of the profile.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user this profile belongs to.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email address of the user this profile belongs to.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this is a kids profile.</summary>
        public bool? IsKids { get; set; }

        /// <summary>Gets or sets the optional avatar icon URL or identifier.</summary>
        public string? AvatarIcon { get; set; }

        /// <summary>Gets or sets the preferred language code (e.g. "en", "ar").</summary>
        public string? LanguagePref { get; set; }

        /// <summary>Gets or sets the maturity level threshold for content filtering.</summary>
        public short? MaturityLevel { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this profile was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this profile was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this profile has been soft-deleted.</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this profile was soft-deleted, if applicable.</summary>
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for profiles returned in the user's profile listing.
    /// </summary>
    public class ProfileListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the profile.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this is a kids profile.</summary>
        public bool? IsKids { get; set; }

        /// <summary>Gets or sets the optional avatar icon URL or identifier.</summary>
        public string? AvatarIcon { get; set; }
    }

    /// <summary>
    /// Request body for creating a new profile.
    /// </summary>
    public class CreateProfileDto
    {
        /// <summary>Gets or sets the display name of the profile. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this is a kids profile.</summary>
        public bool? IsKids { get; set; }

        /// <summary>Gets or sets the optional avatar icon URL. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "AvatarIcon cannot exceed 100 characters.")]
        public string? AvatarIcon { get; set; }

        /// <summary>Gets or sets the preferred language code. Maximum 10 characters.</summary>
        [MaxLength(10, ErrorMessage = "LanguagePref cannot exceed 10 characters.")]
        public string? LanguagePref { get; set; }

        /// <summary>Gets or sets the maturity level threshold (0-4).</summary>
        [Range(0, 4, ErrorMessage = "MaturityLevel must be between 0 and 4.")]
        public short? MaturityLevel { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing profile.
    /// </summary>
    public class UpdateProfileDto
    {
        /// <summary>Gets or sets the display name of the profile. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether this is a kids profile.</summary>
        public bool? IsKids { get; set; }

        /// <summary>Gets or sets the optional avatar icon URL. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "AvatarIcon cannot exceed 100 characters.")]
        public string? AvatarIcon { get; set; }

        /// <summary>Gets or sets the preferred language code. Maximum 10 characters.</summary>
        [MaxLength(10, ErrorMessage = "LanguagePref cannot exceed 10 characters.")]
        public string? LanguagePref { get; set; }

        /// <summary>Gets or sets the maturity level threshold (0-4).</summary>
        [Range(0, 4, ErrorMessage = "MaturityLevel must be between 0 and 4.")]
        public short? MaturityLevel { get; set; }
    }

    /// <summary>
    /// Response returned when successfully switching the active profile.
    /// </summary>
    public class SwitchProfileResponseDto
    {
        /// <summary>Gets or sets the ID of the newly active profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the name of the newly active profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the new JWT token containing the updated ProfileId claim.</summary>
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// Admin list-item DTO for profiles shown in the admin dashboard.
    /// </summary>
    public class AdminProfileListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the profile.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user this profile belongs to.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the preferred language code.</summary>
        public string? LanguagePref { get; set; }

        /// <summary>Gets or sets the maturity level threshold.</summary>
        public short? MaturityLevel { get; set; }

        /// <summary>Gets or sets a value indicating whether this is a kids profile.</summary>
        public bool? IsKids { get; set; }

        /// <summary>Gets or sets the UTC timestamp when this profile was created.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this profile has been soft-deleted.</summary>
        public bool? IsDeleted { get; set; }
        public string? AvatarIcon { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

