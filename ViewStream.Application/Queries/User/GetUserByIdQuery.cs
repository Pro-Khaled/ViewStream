using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.User
{
    public record GetUserByIdQuery(long UserId) : IRequest<UserDto?>;

}
