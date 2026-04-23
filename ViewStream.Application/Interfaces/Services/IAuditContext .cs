using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services
{
    public interface IAuditContext
    {
        string? TableName { get; set; }
        long? RecordId { get; set; }
        string? Action { get; set; }
        object? OldValues { get; set; }
        object? NewValues { get; set; }
        long? ChangedByUserId { get; set; }

        bool HasData { get; }
        void Clear();
    }
}
