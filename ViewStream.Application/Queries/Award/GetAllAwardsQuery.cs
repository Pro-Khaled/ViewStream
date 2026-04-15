using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Award
{
    public record GetAllAwardsQuery : IRequest<List<AwardListItemDto>>;

    public record GetAwardsPagedQuery(int Page = 1, int PageSize = 20, string? SearchTerm = null, int? Year = null) : IRequest<PagedResult<AwardListItemDto>>;

}
