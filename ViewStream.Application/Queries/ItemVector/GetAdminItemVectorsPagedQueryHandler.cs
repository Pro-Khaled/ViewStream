using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ItemVector
{
    // Minimal handler stub to unblock ViewStream.API compilation.
    // TODO: Implement real admin item-vectors paging (filters/sorting/db query).
    public class GetAdminItemVectorsPagedQueryHandler
        : IRequestHandler<GetAdminItemVectorsPagedQuery, PagedResult<AdminItemVectorListItemDto>>
    {
        public Task<PagedResult<AdminItemVectorListItemDto>> Handle(
            GetAdminItemVectorsPagedQuery request,
            CancellationToken cancellationToken)
        {
            var result = new PagedResult<AdminItemVectorListItemDto>
            {
                Items = new List<AdminItemVectorListItemDto>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            };

            return Task.FromResult(result);
        }
    }
}
