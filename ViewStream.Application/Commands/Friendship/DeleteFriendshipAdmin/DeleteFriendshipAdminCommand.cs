using MediatR;

namespace ViewStream.Application.Commands.Friendship.DeleteFriendshipAdmin
{
    public record DeleteFriendshipAdminCommand(long UserId, long FriendId, long AdminUserId) : IRequest<bool>;
}
