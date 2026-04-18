using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class EmailPreferenceDto
    {
        public long UserId { get; set; }
        public bool MarketingEmails { get; set; }
        public bool NewReleaseAlerts { get; set; }
        public bool RecommendationEmails { get; set; }
        public bool AccountUpdates { get; set; }
    }
    public class UpdateEmailPreferenceDto
    {
        public bool? MarketingEmails { get; set; }
        public bool? NewReleaseAlerts { get; set; }
        public bool? RecommendationEmails { get; set; }
        public bool? AccountUpdates { get; set; }
    }

}
