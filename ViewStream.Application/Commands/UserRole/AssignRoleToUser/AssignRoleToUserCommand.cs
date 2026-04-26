using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserRole.AssignRoleToUser
{
    public record AssignRoleToUserCommand(long UserId, AssignRoleToUserDto Dto, long ActorUserId)
        : IRequest<UserRoleDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
