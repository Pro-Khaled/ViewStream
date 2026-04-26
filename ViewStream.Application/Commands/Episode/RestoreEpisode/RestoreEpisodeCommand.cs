using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Episode.RestoreEpisode
{
    public record RestoreEpisodeCommand(long Id, long RestoredByUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => RestoredByUserId;
    }
}
