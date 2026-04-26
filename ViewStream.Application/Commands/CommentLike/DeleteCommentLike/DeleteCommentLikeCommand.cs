using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.CommentLike.DeleteCommentLike
{
    public record DeleteCommentLikeCommand(long CommentId, long ProfileId, long UserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

