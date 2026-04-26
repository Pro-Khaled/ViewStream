using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Role.DeleteRole
{
    public record DeleteRoleCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
