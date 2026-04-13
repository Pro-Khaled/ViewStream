using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentReport.UpdateContentReport
{
    public record UpdateContentReportStatusCommand(long ReportId, UpdateContentReportStatusDto Dto) : IRequest<ContentReportDto?>;
}
