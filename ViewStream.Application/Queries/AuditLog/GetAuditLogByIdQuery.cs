using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.AuditLog
{
    public record GetAuditLogByIdQuery(long Id) : IRequest<AuditLogDto?>;

}
