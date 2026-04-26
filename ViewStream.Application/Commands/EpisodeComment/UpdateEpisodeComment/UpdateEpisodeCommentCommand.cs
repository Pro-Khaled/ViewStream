using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment
{
    public record UpdateEpisodeCommentCommand(long CommentId, long ProfileId, UpdateEpisodeCommentDto Dto, long ActorUserId)
        : IRequest<EpisodeCommentDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
