using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; } // Not present in Genre but kept for consistency
        public int ShowCount { get; set; }
    }


    public class GenreListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ShowCount { get; set; }
    }

    public class CreateGenreDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateGenreDto
    {
        public string Name { get; set; } = string.Empty;
    }

}
