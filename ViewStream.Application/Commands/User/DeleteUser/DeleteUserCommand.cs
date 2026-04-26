using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.User.DeleteUser
{
    // Admin: Soft delete user
    public record DeleteUserCommand(long UserId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
