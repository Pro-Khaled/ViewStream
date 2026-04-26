using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.CommentLike.CreateCommentLike
{
    public record UpsertCommentLikeCommand(long ProfileId, CreateUpdateCommentLikeDto Dto, long UserId)
        : IRequest<CommentLikeDto>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}

