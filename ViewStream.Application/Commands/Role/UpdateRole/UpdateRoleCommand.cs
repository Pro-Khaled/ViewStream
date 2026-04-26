using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Role.UpdateRole
{
    public record UpdateRoleCommand(long Id, UpdateRoleDto Dto, long ActorUserId)
        : IRequest<RoleDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
