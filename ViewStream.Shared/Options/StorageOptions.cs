using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class StorageOptions
    {
        public const string SectionName = "Storage";
        public string RootPath { get; set; } = "./uploads";
        public string Provider { get; set; } = "Local";
    }
}
