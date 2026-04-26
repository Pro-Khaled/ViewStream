using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentTag.UpdateContentTag
{
    public record UpdateContentTagCommand(int Id, UpdateContentTagDto Dto, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
