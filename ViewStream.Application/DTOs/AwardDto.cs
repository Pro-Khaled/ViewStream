using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class AwardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public short? Year { get; set; }
        public int PersonAwardCount { get; set; }
        public int ShowAwardCount { get; set; }
    }

    public class AwardListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public short? Year { get; set; }
    }

    public class CreateAwardDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public short? Year { get; set; }
    }

    public class UpdateAwardDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public short? Year { get; set; }
    }

}
