using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Episode.UpdateEpisode
{
    public record UpdateEpisodeCommand(long Id, UpdateEpisodeDto Dto, long UpdatedByUserId) : IRequest<bool>;

}
