using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions
{
    public record RevokeAllUserSessionsCommand(long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
