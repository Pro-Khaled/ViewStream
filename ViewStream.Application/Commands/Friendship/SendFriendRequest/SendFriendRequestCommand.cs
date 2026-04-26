using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Friendship.SendFriendRequest
{
    public record SendFriendRequestCommand(long UserId, FriendRequestDto Dto, long ActorUserId)
        : IRequest<FriendshipDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
