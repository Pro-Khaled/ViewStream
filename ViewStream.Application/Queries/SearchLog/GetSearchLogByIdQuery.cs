using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.SearchLog
{
    public record GetSearchLogByIdQuery(long Id) : IRequest<SearchLogDto?>;

}
