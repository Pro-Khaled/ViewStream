using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Episode.CreateEpisode
{
    public record CreateEpisodeCommand(CreateEpisodeDto Dto, long CreatedByUserId) : IRequest<long>;

}
