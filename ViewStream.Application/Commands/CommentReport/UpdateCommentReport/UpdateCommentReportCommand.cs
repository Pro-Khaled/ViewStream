using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.CommentReport.UpdateCommentReport
{
    public record UpdateReportStatusCommand(long ReportId, UpdateReportStatusDto Dto, long ReviewedByUserId) : IRequest<CommentReportDto?>;

}
