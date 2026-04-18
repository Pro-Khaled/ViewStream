using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Permission.CreatePermission
{
    public record CreatePermissionCommand(CreatePermissionDto Dto) : IRequest<PermissionDto>;

}
