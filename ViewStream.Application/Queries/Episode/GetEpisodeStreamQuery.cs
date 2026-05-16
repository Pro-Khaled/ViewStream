using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Episode
{
    public record GetEpisodeStreamQuery(long Id) : IRequest<EpisodeStreamUrlDto?>;
}
