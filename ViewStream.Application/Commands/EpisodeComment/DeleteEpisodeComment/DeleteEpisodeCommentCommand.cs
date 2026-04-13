using MediatR;

namespace ViewStream.Application.Commands.EpisodeComment.DeleteEpisodeComment
{
    public record DeleteEpisodeCommentCommand(long Id, long ProfileId, bool IsAdmin = false) : IRequest<bool>; // Soft delete

}
