using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentTag.CreateContentTag
{
    public record CreateContentTagCommand(CreateContentTagDto Dto, long UserId)
        : IRequest<int>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
