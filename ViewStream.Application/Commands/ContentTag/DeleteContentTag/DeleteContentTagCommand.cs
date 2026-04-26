using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.ContentTag.DeleteContentTag
{
    public record DeleteContentTagCommand(int Id, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
