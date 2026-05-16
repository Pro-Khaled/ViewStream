using MediatR;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.EpisodeComment.RestoreEpisodeComment
{
    using EpisodeComment = ViewStream.Domain.Entities.EpisodeComment;

    /// <summary>
    /// Admin command to restore a soft-deleted episode comment.
    /// </summary>
    public record RestoreEpisodeCommentCommand(long Id, long RestoredByUserId)
        : IRequest<bool>, IHasUserId
    {
        /// <inheritdoc />
        long? IHasUserId.UserId => RestoredByUserId;
    }
}
