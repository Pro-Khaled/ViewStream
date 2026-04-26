using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Subtitle.CreateSubtitle
{
    public record CreateSubtitleCommand(CreateSubtitleDto Dto, long ActorUserId)
        : IRequest<long>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
