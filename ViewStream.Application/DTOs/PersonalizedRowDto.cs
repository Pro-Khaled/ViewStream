using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PersonalizedRowDto
    {
        public long ProfileId { get; set; }
        public string RowName { get; set; } = string.Empty;
        public List<long> ShowIds { get; set; } = new();
        public DateTime? GeneratedAt { get; set; }
    }
    public class CreateUpdatePersonalizedRowDto
    {
        public string RowName { get; set; } = string.Empty;
        public List<long> ShowIds { get; set; } = new();
    }
    
}
