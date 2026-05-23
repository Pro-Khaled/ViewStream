using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a cast or crew credit on a show, season, or episode.
    /// </summary>
    public class CreditDto
    {
        /// <summary>Gets or sets the unique identifier of the credit.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the person credited.</summary>
        public long PersonId { get; set; }

        /// <summary>Gets or sets the name of the person credited.</summary>
        public string PersonName { get; set; } = string.Empty;

        /// <summary>Gets or sets the URL to the person's photo.</summary>
        public string? PersonPhotoUrl { get; set; }

        /// <summary>Gets or sets the optional ID of the show.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the optional ID of the season.</summary>
        public long? SeasonId { get; set; }

        /// <summary>Gets or sets the title of the season.</summary>
        public string? SeasonTitle { get; set; }

        /// <summary>Gets or sets the season number.</summary>
        public short? SeasonNumber { get; set; }

        /// <summary>Gets or sets the optional ID of the episode.</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode.</summary>
        public string? EpisodeTitle { get; set; }

        /// <summary>Gets or sets the episode number.</summary>
        public short? EpisodeNumber { get; set; }

        /// <summary>Gets or sets the role of the person (e.g. "Director", "Actor", "Writer").</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Gets or sets the character name, if the role is an actor.</summary>
        public string? CharacterName { get; set; }

        /// <summary>Gets or sets the type of target credited ("Show", "Season", "Episode").</summary>
        public string TargetType { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the target credited.</summary>
        public string TargetTitle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Slim list-item DTO for displaying a credit inline on a show or episode page.
    /// </summary>
    public class CreditListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the credit.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the person credited.</summary>
        public long PersonId { get; set; }

        /// <summary>Gets or sets the name of the person credited.</summary>
        public string PersonName { get; set; } = string.Empty;

        /// <summary>Gets or sets the URL to the person's photo.</summary>
        public string? PersonPhotoUrl { get; set; }

        /// <summary>Gets or sets the role of the person.</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Gets or sets the character name, if applicable.</summary>
        public string? CharacterName { get; set; }

        /// <summary>Gets or sets the type of target credited.</summary>
        public string TargetType { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the target credited.</summary>
        public string TargetTitle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for adding a new credit.
    /// </summary>
    public class CreateCreditDto
    {
        /// <summary>Gets or sets the ID of the person being credited.</summary>
        [Required(ErrorMessage = "PersonId is required.")]
        public long PersonId { get; set; }

        /// <summary>Gets or sets the optional ID of the show.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the optional ID of the season.</summary>
        public long? SeasonId { get; set; }

        /// <summary>Gets or sets the optional ID of the episode.</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets the role of the person. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Role is required.")]
        [MaxLength(100, ErrorMessage = "Role cannot exceed 100 characters.")]
        public string Role { get; set; } = string.Empty;

        /// <summary>Gets or sets the character name. Maximum 200 characters.</summary>
        [MaxLength(200, ErrorMessage = "CharacterName cannot exceed 200 characters.")]
        public string? CharacterName { get; set; }
    }

    /// <summary>
    /// Request body for updating an existing credit.
    /// </summary>
    public class UpdateCreditDto
    {
        /// <summary>Gets or sets the updated role of the person. Maximum 100 characters.</summary>
        [Required(ErrorMessage = "Role is required.")]
        [MaxLength(100, ErrorMessage = "Role cannot exceed 100 characters.")]
        public string Role { get; set; } = string.Empty;

        /// <summary>Gets or sets the updated character name. Maximum 200 characters.</summary>
        [MaxLength(200, ErrorMessage = "CharacterName cannot exceed 200 characters.")]
        public string? CharacterName { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for credits shown in the admin dashboard.
    /// </summary>
    public class AdminCreditListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the credit.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the person credited.</summary>
        public long PersonId { get; set; }

        /// <summary>Gets or sets the name of the person credited.</summary>
        public string? PersonName { get; set; }

        /// <summary>Gets or sets the URL to the person's photo.</summary>
        public string? PersonPhotoUrl { get; set; }

        /// <summary>Gets or sets the role of the person.</summary>
        public string? Role { get; set; }

        /// <summary>Gets or sets the character name, if applicable.</summary>
        public string? CharacterName { get; set; }

        /// <summary>Gets or sets the ID of the show (if this credit targets a show).</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets the ID of the season (if this credit targets a season).</summary>
        public long? SeasonId { get; set; }

        /// <summary>Gets or sets the ID of the episode (if this credit targets an episode).</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets the type of target credited.</summary>
        public string? TargetType { get; set; }

        /// <summary>Gets or sets the title of the target credited.</summary>
        public string? TargetTitle { get; set; }
    }
}
