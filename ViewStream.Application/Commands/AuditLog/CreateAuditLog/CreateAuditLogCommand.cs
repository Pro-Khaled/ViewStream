using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.AuditLog.CreateAuditLog
{
    public record CreateAuditLogCommand(CreateAuditLogDto Dto) : IRequest<AuditLogDto>;

}
