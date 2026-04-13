using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Episode
{
    public record GetEpisodesBySeasonQuery(long SeasonId, bool IncludeDeleted = false) : IRequest<List<EpisodeListItemDto>>;

}
