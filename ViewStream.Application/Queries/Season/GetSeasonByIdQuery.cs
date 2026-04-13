using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Season
{
    public record GetSeasonByIdQuery(long Id) : IRequest<SeasonDto?>;

}
