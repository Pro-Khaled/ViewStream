using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.LoginSession.RevokeLoginSessionAdmin
{
    public record RevokeLoginSessionAdminCommand(long Id, long AdminUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => AdminUserId;
    }
}
