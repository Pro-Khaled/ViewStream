using MediatR;

namespace ViewStream.Application.Commands.CommentLike.DeleteCommentLike
{
    public record DeleteCommentLikeCommand(long CommentId, long ProfileId) : IRequest<bool>;

}
