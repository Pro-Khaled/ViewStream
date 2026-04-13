using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment
{
    public record CreateEpisodeCommentCommand(long ProfileId, CreateEpisodeCommentDto Dto) : IRequest<EpisodeCommentDto>;

}
