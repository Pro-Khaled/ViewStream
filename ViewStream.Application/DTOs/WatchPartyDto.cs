using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a watch party instance.
    /// </summary>
    public class WatchPartyDto
    {
        /// <summary>Gets or sets the unique identifier of the watch party.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that is hosting the watch party.</summary>
        public long HostProfileId { get; set; }

        /// <summary>Gets or sets the display name of the host profile.</summary>
        public string HostProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the episode being watched.</summary>
        public long EpisodeId { get; set; }

        /// <summary>Gets or sets the title of the episode being watched.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the show being watched.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the unique shareable code for joining the party.</summary>
        public string PartyCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when the party started.</summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the party ended.</summary>
        public DateTime? EndedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the party is currently active.</summary>
        public bool? IsActive { get; set; }

        /// <summary>Gets or sets the current number of participants.</summary>
        public int ParticipantCount { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a watch party.
    /// </summary>
    public class WatchPartyListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the watch party.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the host profile.</summary>
        public string HostProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the episode being watched.</summary>
        public string EpisodeTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the show being watched.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the unique shareable code for joining the party.</summary>
        public string PartyCode { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether the party is currently active.</summary>
        public bool? IsActive { get; set; }

        /// <summary>Gets or sets the current number of participants.</summary>
        public int ParticipantCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the party started.</summary>
        public DateTime? StartedAt { get; set; }
    }

    /// <summary>
    /// Request body for creating a new watch party.
    /// </summary>
    public class CreateWatchPartyDto
    {
        /// <summary>Gets or sets the ID of the episode to watch.</summary>
        [Required(ErrorMessage = "EpisodeId is required.")]
        public long EpisodeId { get; set; }
    }

    /// <summary>
    /// Request body for updating the status of a watch party.
    /// </summary>
    public class UpdateWatchPartyDto
    {
        /// <summary>Gets or sets a value indicating whether the party is currently active.</summary>
        public bool? IsActive { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the party ended.</summary>
        public DateTime? EndedAt { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for watch parties shown in the admin dashboard.
    /// </summary>
    public class AdminWatchPartyListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the watch party.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the host profile.</summary>
        public string? HostProfileName { get; set; }

        /// <summary>Gets or sets the title of the episode being watched.</summary>
        public string? EpisodeTitle { get; set; }

        /// <summary>Gets or sets the title of the show being watched.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the unique shareable code for joining the party.</summary>
        public string? PartyCode { get; set; }

        /// <summary>Gets or sets a value indicating whether the party is currently active.</summary>
        public bool? IsActive { get; set; }

        /// <summary>Gets or sets the current number of participants.</summary>
        public int ParticipantCount { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the party started.</summary>
        public DateTime? StartedAt { get; set; }
        public long HostProfileId { get; set; }
        public long EpisodeId { get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

