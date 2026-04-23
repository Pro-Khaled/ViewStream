using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Interfaces.Services
{
    public interface ISystemLogService
    {
        Task LogAuditAsync(CreateAuditLogDto dto, CancellationToken cancellationToken = default);
        Task LogErrorAsync(CreateErrorLogDto dto, CancellationToken cancellationToken = default);
        Task LogSearchAsync(long? profileId, CreateSearchLogDto dto, CancellationToken cancellationToken = default);
    }
}
