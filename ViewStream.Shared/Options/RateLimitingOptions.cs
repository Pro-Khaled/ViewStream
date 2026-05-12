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

        public int PublicReadPermitLimit { get; set; } = 30;
        public int PublicReadWindowSeconds { get; set; } = 60;

        public int UserWritePermitLimit { get; set; } = 30;
        public int UserWriteWindowSeconds { get; set; } = 60;

        public int ReportPermitLimit { get; set; } = 30;
        public int ReportWindowSeconds { get; set; } = 60;

        public int AdminPermitLimit { get; set; } = 50;
        public int AdminWindowSeconds { get; set; } = 60;

        public int AnalyticsPermitLimit { get; set; } = 200;
        public int AnalyticsWindowSeconds { get; set; } = 60;

        public int ContentManagementPermitLimit { get; set; } = 30;
        public int ContentManagementWindowSeconds { get; set; } = 60;

        public int DefaultRateLimit { get; set; } = 20;
        public int DefaultRateLimitWindowSeconds { get; set; } = 60;



        // add more policies as needed
    }
}
