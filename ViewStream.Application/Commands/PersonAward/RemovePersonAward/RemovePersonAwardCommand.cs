using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.PersonAward.RemovePersonAward
{
    public record RemovePersonAwardCommand(long PersonId, int AwardId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
