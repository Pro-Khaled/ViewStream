using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Friendship.BlockUser
{
    public record BlockUserCommand(long UserId, long FriendId, long ActorUserId)
        : IRequest<FriendshipDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
