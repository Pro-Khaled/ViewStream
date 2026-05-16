using MediatR;

namespace ViewStream.Application.Commands.PlaybackEvent.DeletePlaybackEvent
{
    public record DeletePlaybackEventCommand(long Id, long AdminUserId) : IRequest<bool>;
}
