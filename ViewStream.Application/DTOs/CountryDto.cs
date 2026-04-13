using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{

    public class CountryDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Continent { get; set; }
        public int AvailabilityCount { get; set; }
    }

    public class CountryListItemDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Continent { get; set; }
    }
    public class CreateCountryDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Continent { get; set; }
    }

    public class UpdateCountryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Continent { get; set; }
    }

}
