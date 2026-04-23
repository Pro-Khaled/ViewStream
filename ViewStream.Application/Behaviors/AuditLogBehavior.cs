using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Behaviors
{
    public class AuditLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IAuditContext _auditContext;
        private readonly ISystemLogService _systemLog;
        private readonly ILogger<AuditLogBehavior<TRequest, TResponse>> _logger;

        public AuditLogBehavior(
            IAuditContext auditContext,
            ISystemLogService systemLog,
            ILogger<AuditLogBehavior<TRequest, TResponse>> logger)
        {
            _auditContext = auditContext;
            _systemLog = systemLog;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (_auditContext.HasData)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var dto = new CreateAuditLogDto
                        {
                            TableName = _auditContext.TableName!,
                            RecordId = _auditContext.RecordId!.Value,
                            Action = _auditContext.Action!,
                            OldValues = _auditContext.OldValues != null ? JsonSerializer.Serialize(_auditContext.OldValues) : null,
                            NewValues = _auditContext.NewValues != null ? JsonSerializer.Serialize(_auditContext.NewValues) : null,
                            ChangedByUserId = _auditContext.ChangedByUserId
                        };
                        await _systemLog.LogAuditAsync(dto, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to write audit log");
                    }
                    finally
                    {
                        _auditContext.Clear();
                    }
                });
            }

            return response;
        }
    }
}
