using MediatR;

namespace ViewStream.Application.Commands.AuditLog.PurgeOldAuditLogs
{
    public record PurgeOldAuditLogsCommand(int DaysToKeep, long AdminUserId) : IRequest<int>;
}
