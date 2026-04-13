using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class ShowAvailabilityDto
    {
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public DateOnly? AvailableFrom { get; set; }
        public DateOnly? AvailableUntil { get; set; }
        public string? LicensingNotes { get; set; }
    }

    public class ShowAvailabilityListItemDto
    {
        public long ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public DateOnly? AvailableFrom { get; set; }
        public DateOnly? AvailableUntil { get; set; }
    }

    public class CreateShowAvailabilityDto
    {
        public long ShowId { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public DateOnly? AvailableFrom { get; set; }
        public DateOnly? AvailableUntil { get; set; }
        public string? LicensingNotes { get; set; }
    }

    public class UpdateShowAvailabilityDto
    {
        public DateOnly? AvailableFrom { get; set; }
        public DateOnly? AvailableUntil { get; set; }
        public string? LicensingNotes { get; set; }
    }


}
