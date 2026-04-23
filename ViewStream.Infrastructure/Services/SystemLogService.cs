using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Commands.AuditLog.CreateAuditLog;
using ViewStream.Application.Commands.ErrorLog.CreateErrorLog;
using ViewStream.Application.Commands.SearchLog.CreateSearchLog;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services
{
    public class SystemLogService : ISystemLogService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SystemLogService> _logger;

        public SystemLogService(IMediator mediator, ILogger<SystemLogService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task LogAuditAsync(CreateAuditLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.Send(new CreateAuditLogCommand(dto), cancellationToken);
            }
            catch (Exception ex)
            {
                // Fallback to standard logging – do not rethrow
                _logger.LogError(ex, "Failed to write audit log for table {TableName}, record {RecordId}", dto.TableName, dto.RecordId);
            }
        }

        public async Task LogErrorAsync(CreateErrorLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.Send(new CreateErrorLogCommand(dto), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write error log for endpoint {Endpoint}", dto.Endpoint);
            }
        }

        public async Task LogSearchAsync(long? profileId, CreateSearchLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.Send(new CreateSearchLogCommand(profileId, dto), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write search log for query '{Query}'", dto.Query);
            }
        }
    }
}
