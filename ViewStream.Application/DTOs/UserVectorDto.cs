using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class UserVectorDto
    {
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string? EmbeddingJson { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class CreateUpdateUserVectorDto
    {
        public string? EmbeddingJson { get; set; }
    }

}
