using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.User
{
    public record GetUsersPagedQuery(int Page = 1, int PageSize = 20, string? SearchTerm = null)
        : IRequest<PagedResult<UserDto>>;
}
