using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Permission.UpdatePermission
{
    public record UpdatePermissionCommand(int Id, UpdatePermissionDto Dto) : IRequest<PermissionDto?>;

}
