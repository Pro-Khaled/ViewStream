using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentReport.CreateContentReport
{
    public record CreateContentReportCommand(long ProfileId, CreateContentReportDto Dto) : IRequest<ContentReportDto>;
}
