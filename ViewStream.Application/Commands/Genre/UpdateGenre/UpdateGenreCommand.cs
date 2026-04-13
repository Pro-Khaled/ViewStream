using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Genre.UpdateGenre
{
    public record UpdateGenreCommand(int Id, UpdateGenreDto Dto) : IRequest<bool>;

}
