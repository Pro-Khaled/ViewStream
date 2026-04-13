using MediatR;

namespace ViewStream.Application.Commands.Episode.RestoreEpisode
{
    public record RestoreEpisodeCommand(long Id, long RestoredByUserId) : IRequest<bool>;

}
