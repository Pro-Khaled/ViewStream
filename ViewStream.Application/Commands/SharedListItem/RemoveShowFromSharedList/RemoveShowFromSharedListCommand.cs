using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList
{
    public record RemoveShowFromSharedListCommand(long ListId, long ShowId, long ProfileId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
