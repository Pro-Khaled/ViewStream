using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class SearchLogDto
    {
        public long Id { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileName { get; set; }
        public string Query { get; set; } = string.Empty;
        public int? ResultsCount { get; set; }
        public long? ClickedShowId { get; set; }
        public string? ClickedShowTitle { get; set; }
        public DateTime? SearchAt { get; set; }
    }
    public class SearchLogListItemDto
    {
        public long Id { get; set; }
        public string? ProfileName { get; set; }
        public string Query { get; set; } = string.Empty;
        public int? ResultsCount { get; set; }
        public string? ClickedShowTitle { get; set; }
        public DateTime? SearchAt { get; set; }
    }
    public class CreateSearchLogDto
    {
        public string Query { get; set; } = string.Empty;
        public int? ResultsCount { get; set; }
        public long? ClickedShowId { get; set; }
    }

}
