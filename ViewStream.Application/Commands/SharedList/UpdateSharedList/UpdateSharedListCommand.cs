using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.SharedList.UpdateSharedList
{
    public record UpdateSharedListCommand(long Id, long OwnerProfileId, UpdateSharedListDto Dto, long ActorUserId)
        : IRequest<SharedListDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
