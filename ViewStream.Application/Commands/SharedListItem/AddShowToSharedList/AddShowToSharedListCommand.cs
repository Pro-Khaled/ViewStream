using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.SharedListItem.AddShowToSharedList
{
    public record AddShowToSharedListCommand(long ListId, long ProfileId, AddShowToSharedListDto Dto, long ActorUserId)
        : IRequest<SharedListItemDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
