using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class SwaggerOptions
    {
        public const string SectionName = "Swagger";

        public string Title { get; set; } = "API";
        public string Version { get; set; } = "v5";
        public string Description { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactUrl { get; set; } = string.Empty;
        public string LicenseName { get; set; } = "MIT License";
        public string LicenseUrl { get; set; } = "https://opensource.org/licenses/MIT";
        public string TermsOfServiceUrl { get; set; } = string.Empty;
        public bool EnableJwtAuthentication { get; set; } = true;
        public bool EnableXmlComments { get; set; } = false;
        public bool EnableTryItOutByDefault { get; set; } = true;
        public bool EnableDeepLinking { get; set; } = false;
        public bool EnableDisplayRequestDuration { get; set; } = true;
        public int DefaultModelsExpandDepth { get; set; } = -1;
        public string RoutePrefix { get; set; } = "swagger";
    }
}
