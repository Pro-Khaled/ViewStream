using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Friendship.Unfriend
{
    public record UnfriendCommand(long UserId, long FriendId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
