using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Invoice
{
    /// <summary>
    /// Admin paged invoice query supports filtering by user/status/date range and sorting.
    /// </summary>
    public record GetAdminInvoicesPagedQuery(
        int pageNumber = 1,
        int pageSize = 20,
        long? userId = null,
        string? status = null,
        DateOnly? from = null,
        DateOnly? to = null,
        string? sortBy = "invoiceDate",
        bool sortDescending = true,
        bool includeDeleted = false,
        string? searchTerm = null)
        : AdminPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted),
          IRequest<PagedResult<AdminInvoiceListItemDto>>
    {
        /// <summary>
        /// Optional filter by user id.
        /// </summary>
        public long? UserId { get; init; } = userId;

        /// <summary>
        /// Optional filter by exact status.
        /// </summary>
        public string? Status { get; init; } = status;

        /// <summary>
        /// Optional filter by invoice date start (inclusive).
        /// </summary>
        public DateOnly? From { get; init; } = from;

        /// <summary>
        /// Optional filter by invoice date end (inclusive).
        /// </summary>
        public DateOnly? To { get; init; } = to;

        /// <summary>
        /// The requested sort field (InvoiceDate/Amount/Status).
        /// </summary>
        public string? SortField { get; init; } = sortBy;

        /// <summary>
        /// Whether to sort descending.
        /// </summary>
        public bool SortDesc { get; init; } = sortDescending;
    }
}
