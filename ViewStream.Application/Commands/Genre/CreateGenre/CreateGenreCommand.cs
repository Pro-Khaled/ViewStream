using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Genre.CreateGenre
{
    public record CreateGenreCommand(CreateGenreDto Dto) : IRequest<int>;

}
