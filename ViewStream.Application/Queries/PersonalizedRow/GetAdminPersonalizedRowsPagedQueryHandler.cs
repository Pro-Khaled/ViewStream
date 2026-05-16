using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PersonalizedRow
{
    public class GetAdminPersonalizedRowsPagedQueryHandler
        : IRequestHandler<GetAdminPersonalizedRowsPagedQuery, PagedResult<AdminPersonalizedRowListItemDto>>
    {
        public Task<PagedResult<AdminPersonalizedRowListItemDto>> Handle(
            GetAdminPersonalizedRowsPagedQuery request,
            CancellationToken cancellationToken)
        {
            var result = new PagedResult<AdminPersonalizedRowListItemDto>
            {
                Items = new List<AdminPersonalizedRowListItemDto>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            };

            return Task.FromResult(result);
        }
    }
}
