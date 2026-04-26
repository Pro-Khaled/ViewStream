using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.User.UnblockUser
{
    // Admin: Unblock user
    public record UnblockUserCommand(long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
