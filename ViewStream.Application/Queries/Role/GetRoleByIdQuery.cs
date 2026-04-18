using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Role
{
    public record GetRoleByIdQuery(long Id) : IRequest<RoleDto?>;

}
