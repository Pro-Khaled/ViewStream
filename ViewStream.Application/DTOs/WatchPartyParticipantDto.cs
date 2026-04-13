using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class WatchPartyParticipantDto
    {
        public long PartyId { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool IsOnline { get; set; }
    }
}
