using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Subtitle
{
    public record GetSubtitlesByEpisodeQuery(long EpisodeId, bool IncludeDeleted = false) : IRequest<List<SubtitleListItemDto>>;
}
