using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.SharedList.DeleteSharedList
{
    public record DeleteSharedListCommand(long Id, long OwnerProfileId, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
