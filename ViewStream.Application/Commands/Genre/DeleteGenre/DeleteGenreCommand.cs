using MediatR;

namespace ViewStream.Application.Commands.Genre.DeleteGenre
{
    public record DeleteGenreCommand(int Id) : IRequest<bool>; 

}
