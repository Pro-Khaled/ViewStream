using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ErrorLog
{
    public record GetErrorLogByIdQuery(long Id) : IRequest<ErrorLogDto?>;

}
