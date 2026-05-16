using MediatR;

namespace ViewStream.Application.Commands.ErrorLog.DeleteErrorLog
{
    public record DeleteErrorLogCommand(long Id, long AdminUserId) : IRequest<bool>;
}
