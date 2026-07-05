using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.User
{
    public record SearchUsersQuery(string Q, int Limit = 20, long? CurrentUserId = null) : IRequest<List<UserPublicSearchResultDto>>;
}
