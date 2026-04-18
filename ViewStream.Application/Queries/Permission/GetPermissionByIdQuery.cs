using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Permission
{
    public record GetPermissionByIdQuery(int Id) : IRequest<PermissionDto?>;

}
