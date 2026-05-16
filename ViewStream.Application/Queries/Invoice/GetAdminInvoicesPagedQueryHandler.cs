using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Invoice
{
    // Minimal handler stub to unblock ViewStream.API compilation.
    // TODO: Implement real admin invoice paging (filters/sorting/db query).
    public class GetAdminInvoicesPagedQueryHandler : IRequestHandler<GetAdminInvoicesPagedQuery, PagedResult<AdminInvoiceListItemDto>>
    {
        public Task<PagedResult<AdminInvoiceListItemDto>> Handle(GetAdminInvoicesPagedQuery request, CancellationToken cancellationToken)
        {
            var result = new PagedResult<AdminInvoiceListItemDto>
            {
                Items = new List<AdminInvoiceListItemDto>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            };

            return Task.FromResult(result);
        }
    }
}
