using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent
{
    public record CreatePlaybackEventCommand(long ProfileId, CreatePlaybackEventDto Dto) : IRequest<PlaybackEventDto>;

}
