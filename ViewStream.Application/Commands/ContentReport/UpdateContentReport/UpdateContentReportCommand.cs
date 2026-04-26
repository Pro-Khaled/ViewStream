using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentReport.UpdateContentReport
{
    public record UpdateContentReportStatusCommand(long ReportId, UpdateContentReportStatusDto Dto, long ReviewedByUserId)
        : IRequest<ContentReportDto?>, IHasUserId
    {
        long? IHasUserId.UserId => ReviewedByUserId;
    }
}
