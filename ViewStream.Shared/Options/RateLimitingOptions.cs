using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";

        public int AuthPermitLimit { get; set; } = 5;
        public int AuthWindowSeconds { get; set; } = 60;

        public int SearchPermitLimit { get; set; } = 30;
        public int SearchWindowSeconds { get; set; } = 60;

        // add more policies as needed
    }
}
