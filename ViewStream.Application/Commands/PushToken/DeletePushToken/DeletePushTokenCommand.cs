using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PushToken.DeletePushToken
{
    public record DeletePushTokenCommand(long Id, long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
