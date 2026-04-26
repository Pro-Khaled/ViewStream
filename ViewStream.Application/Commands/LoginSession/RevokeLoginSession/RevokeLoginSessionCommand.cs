using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.LoginSession.RevokeLoginSession
{
    public record RevokeLoginSessionCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
