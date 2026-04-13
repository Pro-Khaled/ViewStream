using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class SharedListItemDto
    {
        public long ListId { get; set; }
        public string ListName { get; set; } = string.Empty;
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public short? ReleaseYear { get; set; }
        public long? AddedByProfileId { get; set; }
        public string? AddedByProfileName { get; set; }
        public DateTime? AddedAt { get; set; }
    }

    public class SharedListItemListItemDto
    {
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public DateTime? AddedAt { get; set; }
    }
    public class AddShowToSharedListDto
    {
        public long ShowId { get; set; }
    }

}
