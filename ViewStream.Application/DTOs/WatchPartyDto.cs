using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class WatchPartyDto
    {
        public long Id { get; set; }
        public long HostProfileId { get; set; }
        public string HostProfileName { get; set; } = string.Empty;
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public string ShowTitle { get; set; } = string.Empty;
        public string PartyCode { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool? IsActive { get; set; }
        public int ParticipantCount { get; set; }
    }

    public class WatchPartyListItemDto
    {
        public long Id { get; set; }
        public string HostProfileName { get; set; } = string.Empty;
        public string EpisodeTitle { get; set; } = string.Empty;
        public string ShowTitle { get; set; } = string.Empty;
        public string PartyCode { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public int ParticipantCount { get; set; }
        public DateTime? StartedAt { get; set; }
    }

    public class CreateWatchPartyDto
    {
        public long EpisodeId { get; set; }
    }

    public class UpdateWatchPartyDto
    {
        public bool? IsActive { get; set; }
        public DateTime? EndedAt { get; set; }
    }


}
