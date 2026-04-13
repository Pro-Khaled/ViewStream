using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Season
{
    public record GetSeasonsByShowQuery(long ShowId, bool IncludeDeleted = false) : IRequest<List<SeasonListItemDto>>;
}
