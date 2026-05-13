using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class RedisOptions
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; set; } = "localhost:6379";
        public string InstanceName { get; set; } = "ViewStreamCache";
        public int DefaultDatabase { get; set; } = -1; // -1 means use default
    }

}
