using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Episode.CreateEpisode
{
    public record CreateEpisodeCommand(CreateEpisodeDto Dto, long CreatedByUserId)
        : IRequest<long>, IHasUserId
    {
        long? IHasUserId.UserId => CreatedByUserId;
    }
}
