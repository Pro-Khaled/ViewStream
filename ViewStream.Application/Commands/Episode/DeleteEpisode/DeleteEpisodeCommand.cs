using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Episode.DeleteEpisode
{
    public record DeleteEpisodeCommand(long Id, long DeletedByUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => DeletedByUserId;
    }
}
