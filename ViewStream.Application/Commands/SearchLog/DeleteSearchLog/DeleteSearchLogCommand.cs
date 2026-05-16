using MediatR;

namespace ViewStream.Application.Commands.SearchLog.DeleteSearchLog
{
    public record DeleteSearchLogCommand(long Id, long AdminUserId) : IRequest<bool>;
}
