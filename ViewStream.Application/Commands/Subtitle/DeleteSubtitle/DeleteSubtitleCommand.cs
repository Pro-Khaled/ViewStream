using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Subtitle.DeleteSubtitle
{
    public record DeleteSubtitleCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
