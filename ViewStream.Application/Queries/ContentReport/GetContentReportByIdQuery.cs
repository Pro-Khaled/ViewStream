using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentReport
{
    public record GetContentReportByIdQuery(long Id) : IRequest<ContentReportDto?>;

}
