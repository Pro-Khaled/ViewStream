using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.EpisodeComment.DeleteEpisodeComment
{
    public record DeleteEpisodeCommentCommand(long CommentId, long ProfileId, bool IsAdmin, long ActorUserId)
        : IRequest<bool>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
