using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Invoice
{
    public record GetInvoicesPagedQuery(
        int Page = 1,
        int PageSize = 20,
        long? UserId = null,
        string? Status = null,
        DateOnly? From = null,
        DateOnly? To = null,
        string OrderBy = "invoiceDate",
        bool IsDescending = true
    ) : IRequest<PagedResult<InvoiceListItemDto>>;
}
