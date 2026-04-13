using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ContentTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int ShowCount { get; set; }
    }

    public class ContentTagListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int ShowCount { get; set; }
    }

    public class CreateContentTagDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
    }

    public class UpdateContentTagDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
    }


}
