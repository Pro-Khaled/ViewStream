using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.UserRole.RemoveRoleFromUser
{
    public record RemoveRoleFromUserCommand(long UserId, long RoleId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
