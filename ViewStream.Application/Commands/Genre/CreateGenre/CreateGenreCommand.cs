using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Genre.CreateGenre
{
    public record CreateGenreCommand(CreateGenreDto Dto, long UserId)
        : IRequest<int>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
