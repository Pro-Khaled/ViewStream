using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a participant in a watch party.
    /// </summary>
    public class WatchPartyParticipantDto
    {
        /// <summary>Gets or sets the ID of the watch party.</summary>
        public long PartyId { get; set; }

        /// <summary>Gets or sets the ID of the participating profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the participating profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when the participant joined the party.</summary>
        public DateTime? JoinedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the participant left the party.</summary>
        public DateTime? LeftAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the participant is currently online in the party.</summary>
        public bool IsOnline { get; set; }
    }
}
