using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Role.UpdateRole
{
    public record UpdateRoleCommand(long Id, UpdateRoleDto Dto) : IRequest<RoleDto?>;

}
