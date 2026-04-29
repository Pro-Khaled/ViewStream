using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Commands.AuditLog.CreateAuditLog;
using ViewStream.Application.Commands.ErrorLog.CreateErrorLog;
using ViewStream.Application.Commands.SearchLog.CreateSearchLog;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services;

public class SystemLogService : ISystemLogService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SystemLogService> _logger;

    public SystemLogService(IServiceScopeFactory scopeFactory, ILogger<SystemLogService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task LogAuditAsync(CreateAuditLogDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new CreateAuditLogCommand(dto), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log for table {TableName}, record {RecordId}",
                dto.TableName, dto.RecordId);
        }
    }

    public async Task LogErrorAsync(CreateErrorLogDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new CreateErrorLogCommand(dto), cancellationToken);
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
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new CreateSearchLogCommand(profileId, dto), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write search log for query '{Query}'", dto.Query);
        }
    }
}