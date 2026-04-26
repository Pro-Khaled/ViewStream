using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Genre.DeleteGenre
{
    public record DeleteGenreCommand(int Id, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
