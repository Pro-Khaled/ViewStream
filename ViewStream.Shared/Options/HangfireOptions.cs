using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class HangfireOptions
    {
        public const string SectionName = "Hangfire";
        public int CommandBatchMaxTimeoutSeconds { get; set; } = 300;
        public int SlidingInvisibilityTimeoutSeconds { get; set; } = 300;
        public int QueuePollIntervalMilliseconds { get; set; } = 0;
        public bool DisableGlobalLocks { get; set; } = true;
    }
}
