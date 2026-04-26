using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.CommentReport.CreateCommentReport
{
    public record CreateCommentReportCommand(long ProfileId, CreateCommentReportDto Dto, long UserId)
    : IRequest<CommentReportDto>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
