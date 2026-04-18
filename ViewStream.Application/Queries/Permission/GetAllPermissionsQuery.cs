using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Permission
{
    public record GetAllPermissionsQuery : IRequest<List<PermissionListItemDto>>;

}
