using MediatR;

namespace ViewStream.Application.Commands.AuditLog.DeleteAuditLog
{
    public record DeleteAuditLogCommand(long Id, long AdminUserId) : IRequest<bool>;
}
