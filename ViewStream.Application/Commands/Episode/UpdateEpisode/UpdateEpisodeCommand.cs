using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Episode.UpdateEpisode
{
    public record UpdateEpisodeCommand(long Id, UpdateEpisodeDto Dto, long UpdatedByUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UpdatedByUserId;
    }
}
