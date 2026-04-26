using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Permission.DeletePermission
{
    public record DeletePermissionCommand(int Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
