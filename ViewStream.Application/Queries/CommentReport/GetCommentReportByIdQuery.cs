using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.CommentReport
{
    public record GetReportByIdQuery(long Id) : IRequest<CommentReportDto?>;

}
