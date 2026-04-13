using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Episode
{
    public record GetEpisodeByIdQuery(long Id) : IRequest<EpisodeDto?>;

}
