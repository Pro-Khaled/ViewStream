using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class CreditDto
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? PersonPhotoUrl { get; set; }
        public long? ShowId { get; set; }
        public string? ShowTitle { get; set; }
        public long? SeasonId { get; set; }
        public string? SeasonTitle { get; set; }
        public short? SeasonNumber { get; set; }
        public long? EpisodeId { get; set; }
        public string? EpisodeTitle { get; set; }
        public short? EpisodeNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? CharacterName { get; set; }
        public string TargetType { get; set; } = string.Empty; // "Show", "Season", "Episode"
        public string TargetTitle { get; set; } = string.Empty;
    }

    public class CreditListItemDto
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? PersonPhotoUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? CharacterName { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public string TargetTitle { get; set; } = string.Empty;
    }

    public class CreateCreditDto
    {
        public long PersonId { get; set; }
        public long? ShowId { get; set; }
        public long? SeasonId { get; set; }
        public long? EpisodeId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? CharacterName { get; set; }
    }
    public class UpdateCreditDto
    {
        public string Role { get; set; } = string.Empty;
        public string? CharacterName { get; set; }
    }

}
