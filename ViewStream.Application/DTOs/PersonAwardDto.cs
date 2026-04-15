using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PersonAwardDto
    {
        public long PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public int AwardId { get; set; }
        public string AwardName { get; set; } = string.Empty;
        public string? AwardCategory { get; set; }
        public short? AwardYear { get; set; }
        public bool? Won { get; set; }
    }
    public class CreatePersonAwardDto
    {
        public int AwardId { get; set; }
        public bool? Won { get; set; }
    }

}
