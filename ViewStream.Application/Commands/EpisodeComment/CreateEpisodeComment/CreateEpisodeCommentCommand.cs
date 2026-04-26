using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment
{
    public record CreateEpisodeCommentCommand(long ProfileId, CreateEpisodeCommentDto Dto, long ActorUserId)
        : IRequest<EpisodeCommentDto>, IHasUserId
    {
        long? IHasUserId.UserId => ActorUserId;
    }
}
