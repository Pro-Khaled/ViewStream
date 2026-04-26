using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ItemVector.UpsertItemVector
{
    public record UpsertItemVectorCommand(long ShowId, CreateUpdateItemVectorDto Dto, long ActorUserId)
        : IRequest<ItemVectorDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
