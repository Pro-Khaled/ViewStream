using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.SharedList.GenerateShareCode
{
    public record GenerateShareCodeCommand(long Id, long OwnerProfileId, long ActorUserId)
        : IRequest<string?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
