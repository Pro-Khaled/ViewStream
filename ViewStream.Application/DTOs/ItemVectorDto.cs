using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ItemVectorDto
    {
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? EmbeddingJson { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class CreateUpdateItemVectorDto
    {
        public string? EmbeddingJson { get; set; }
    }

}
