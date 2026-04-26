using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Friendship.RespondToFriendRequest
{
    public record RespondToFriendRequestCommand(long UserId, long FriendId, UpdateFriendshipStatusDto Dto, long ActorUserId)
        : IRequest<FriendshipDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
