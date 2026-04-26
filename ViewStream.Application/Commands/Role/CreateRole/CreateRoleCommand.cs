using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Role.CreateRole
{
    public record CreateRoleCommand(CreateRoleDto Dto, long ActorUserId)
        : IRequest<RoleDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
