using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment
{
    public record UpdateEpisodeCommentCommand(long Id, long ProfileId, UpdateEpisodeCommentDto Dto) : IRequest<EpisodeCommentDto?>;

}
