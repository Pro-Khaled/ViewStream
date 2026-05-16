using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Invoice
{
    // Wrapper so ViewStream.API controllers can reference:
    // ViewStream.Application.Queries.Invoice.GetAdminInvoicesPagedQuery
    public record GetAdminInvoicesPagedQuery
        : AdminPagedQuery,IRequest<PagedResult<AdminInvoiceListItemDto>>
    {
        //public GetAdminInvoicesPagedQuery(
        //    int pageNumber = 1,
        //    int pageSize = 20,
        //    long? userId = null,
        //    string? status = null,
        //    DateOnly? from = null,
        //    DateOnly? to = null,
        //    string? sortBy = "invoiceDate",
        //    bool sortDescending = true
        //) : base(
        //    pageNumber,
        //    pageSize,
        //    userId,
        //    status,
        //    from,
        //    to,
        //    orderBy: sortBy ?? "invoiceDate",
        //    sortDescending: sortDescending
        //)
        //{
        //}
    }
}
