using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Invoice
{
    public class GetInvoicesPagedQueryHandler
        : IRequestHandler<GetInvoicesPagedQuery, PagedResult<InvoiceListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetInvoicesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<InvoiceListItemDto>> Handle(
            GetInvoicesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Invoices.GetQueryable();

            if (request.UserId.HasValue)
                query = query.Where(i => i.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(i => i.Status == request.Status);

            if (request.From.HasValue)
                query = query.Where(i => i.InvoiceDate >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(i => i.InvoiceDate <= request.To.Value);

            // Ordering
            query = (request.OrderBy?.ToLower(), request.IsDescending) switch
            {
                ("amount", true)        => query.OrderByDescending(i => i.Amount),
                ("amount", false)       => query.OrderBy(i => i.Amount),
                ("status", true)        => query.OrderByDescending(i => i.Status),
                ("status", false)       => query.OrderBy(i => i.Status),
                (_, true)               => query.OrderByDescending(i => i.InvoiceDate),
                _                       => query.OrderBy(i => i.InvoiceDate)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<InvoiceListItemDto>
            {
                Items = _mapper.Map<List<InvoiceListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
