using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Invoice
{
    using Invoice = ViewStream.Domain.Entities.Invoice;
    /// <summary>
    /// Handles admin paged invoice list queries with optional filters and sorting.
    /// </summary>
    public class GetAdminInvoicesPagedQueryHandler
        : IRequestHandler<GetAdminInvoicesPagedQuery, PagedResult<AdminInvoiceListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAdminInvoicesPagedQueryHandler> _logger;

        public GetAdminInvoicesPagedQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAdminInvoicesPagedQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PagedResult<AdminInvoiceListItemDto>> Handle(
            GetAdminInvoicesPagedQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<Invoice> query = _unitOfWork.Invoices
                    .GetQueryable()
                    .AsNoTracking();

                // IncludeDeleted is ignored because Invoice has no soft-delete columns in this model.
                // (Kept intentionally for consistency with AdminPagedQuery usage.)

                if (request.UserId.HasValue)
                    query = query.Where(i => i.UserId == request.UserId.Value);

                if (!string.IsNullOrWhiteSpace(request.Status))
                    query = query.Where(i => i.Status != null && i.Status == request.Status);

                if (request.From.HasValue)
                    query = query.Where(i => i.InvoiceDate >= request.From.Value);

                if (request.To.HasValue)
                    query = query.Where(i => i.InvoiceDate <= request.To.Value);

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.Trim();
                    // Spec phase-1: search LanguageCode or Episode.Title is for audio tracks; for invoices/export we apply best-effort
                    // search on common text fields.
                    query = query.Where(i =>
                        (i.InvoicePdfUrl != null && i.InvoicePdfUrl.Contains(term)) ||
                        (i.TransactionId != null && i.TransactionId.Contains(term)) ||
                        (i.Status != null && i.Status.Contains(term)));
                }

                // Sorting: InvoiceDate, Amount, Status
                var sortBy = request.SortField?.Trim().ToLowerInvariant();
                query = sortBy switch
                {
                    "amount" => request.SortDesc ? query.OrderByDescending(i => i.Amount) : query.OrderBy(i => i.Amount),
                    "status" => request.SortDesc ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status),
                    _ => request.SortDesc ? query.OrderByDescending(i => i.InvoiceDate) : query.OrderBy(i => i.InvoiceDate)
                };

                var projected = query.ProjectTo<AdminInvoiceListItemDto>(_mapper.ConfigurationProvider);

                var totalCount = await projected.CountAsync(cancellationToken);

                var items = await projected
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                return new PagedResult<AdminInvoiceListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get admin invoices paged result");
                throw;
            }
        }
    }
}
