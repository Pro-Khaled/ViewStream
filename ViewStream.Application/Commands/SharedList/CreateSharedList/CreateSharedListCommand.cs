using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.SharedList.CreateSharedList
{
    public record CreateSharedListCommand(long OwnerProfileId, CreateSharedListDto Dto, long ActorUserId)
        : IRequest<SharedListDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
