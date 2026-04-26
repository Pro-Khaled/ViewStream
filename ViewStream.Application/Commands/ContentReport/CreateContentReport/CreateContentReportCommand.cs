using MediatR;
using ViewStream.Application.Behaviors;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.ContentReport.CreateContentReport
{
    public record CreateContentReportCommand(long ProfileId, CreateContentReportDto Dto, long UserId)
        : IRequest<ContentReportDto>, IHasUserId
    {
        long? IHasUserId.UserId => UserId;
    }
}
