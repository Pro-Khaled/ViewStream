using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Permission.UpdatePermission
{
    public record UpdatePermissionCommand(int Id, UpdatePermissionDto Dto, long ActorUserId)
        : IRequest<PermissionDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
