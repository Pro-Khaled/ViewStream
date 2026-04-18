using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Role.CreateRole
{
    public record CreateRoleCommand(CreateRoleDto Dto) : IRequest<RoleDto>;

}
