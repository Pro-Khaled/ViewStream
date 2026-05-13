using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class CacheOptions
    {
        public const string SectionName = "CacheOptions";

        /// <summary>Default TTL in seconds (300 = 5 minutes)</summary>
        public int DefaultTtlSeconds { get; set; } = 300;

        /// <summary>Optional: enable/disable caching globally</summary>
        public bool Enabled { get; set; } = true;

        // New unified list :
        public List<CacheableQueryConfig> CacheableQueries { get; set; } = new();
    }

    public class CacheableQueryConfig
    {
        public string Name { get; set; } = string.Empty; //List of query type names that should be cached(case-insensitive)
        public int? TtlSeconds { get; set; }  // null means use default
    }
}
