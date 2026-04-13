using MediatR;

namespace ViewStream.Application.Commands.Episode.DeleteEpisode
{
    public record DeleteEpisodeCommand(long Id, long DeletedByUserId) : IRequest<bool>;

}
