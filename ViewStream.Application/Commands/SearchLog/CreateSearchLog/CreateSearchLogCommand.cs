using MediatR;
using ViewStream.Application.DTOs;


namespace ViewStream.Application.Commands.SearchLog.CreateSearchLog
{
    public record CreateSearchLogCommand(long? ProfileId, CreateSearchLogDto Dto) : IRequest<SearchLogDto>;

}
