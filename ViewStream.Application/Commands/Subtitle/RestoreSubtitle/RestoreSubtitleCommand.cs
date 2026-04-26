using MediatR;
using ViewStream.Application.Behaviors;


namespace ViewStream.Application.Commands.Subtitle.RestoreSubtitle
{
    public record RestoreSubtitleCommand(long Id, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
