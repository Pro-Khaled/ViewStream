using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class AppOptions
    {
        public const string SectionName = "App";
        public string BaseUrl { get; set; } = "https://localhost:5001"; // default
    }
}
