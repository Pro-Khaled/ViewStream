using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Rating.DeleteRating
{
    public record DeleteRatingCommand(long ProfileId, long ShowId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
