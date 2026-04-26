using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.ShowAward.RemoveShowAward
{
    public record RemoveShowAwardCommand(long ShowId, int AwardId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
