using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Permission.CreatePermission
{
    public record CreatePermissionCommand(CreatePermissionDto Dto, long ActorUserId)
        : IRequest<PermissionDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
