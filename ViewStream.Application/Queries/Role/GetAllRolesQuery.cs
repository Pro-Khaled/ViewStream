using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Role
{
    public record GetAllRolesQuery : IRequest<List<RoleListItemDto>>;

}
