using MediatR;

namespace ViewStream.Application.Commands.CommentLike.DeleteCommentLikeAdmin
{
    public record DeleteCommentLikeAdminCommand(long CommentId, long ProfileId, long AdminUserId) : IRequest<bool>;
}
