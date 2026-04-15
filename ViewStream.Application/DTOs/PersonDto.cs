using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PersonDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public int CreditCount { get; set; }
        public int AwardCount { get; set; }
    }

    public class PersonListItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public int CreditCount { get; set; }
    }

    public class CreatePersonDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
    }
    public class UpdatePersonDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
    }

}
