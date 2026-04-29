using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Behaviors;

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
            // Fire‑and‑forget, but the SystemLogService creates its own scope, so no DbContext disposal issues
            _ = _systemLog.LogAuditAsync(new CreateAuditLogDto
            {
                TableName = _auditContext.TableName!,
                RecordId = _auditContext.RecordId!.Value,
                Action = _auditContext.Action!,
                OldValues = _auditContext.OldValues != null ? JsonSerializer.Serialize(_auditContext.OldValues) : null,
                NewValues = _auditContext.NewValues != null ? JsonSerializer.Serialize(_auditContext.NewValues) : null,
                ChangedByUserId = _auditContext.ChangedByUserId
            }, cancellationToken);

            // Clear the context once the logging command has been issued
            _auditContext.Clear();
        }

        return response;
    }
}