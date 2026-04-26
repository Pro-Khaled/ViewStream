using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subtitle.UpdateSubtitle
{
    public record UpdateSubtitleCommand(long Id, UpdateSubtitleDto Dto, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
