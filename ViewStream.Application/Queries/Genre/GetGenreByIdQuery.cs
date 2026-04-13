using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Genre
{
    public record GetGenreByIdQuery(int Id) : IRequest<GenreDto?>;

}
