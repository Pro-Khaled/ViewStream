using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.CommentLike.CreateCommentLike
{
    // Upsert: creates or updates a reaction for the current profile
    public record UpsertCommentLikeCommand(long ProfileId, CreateUpdateCommentLikeDto Dto) : IRequest<CommentLikeDto>;
}
