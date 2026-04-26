using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Genre.UpdateGenre
{
    public record UpdateGenreCommand(int Id, UpdateGenreDto Dto, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
