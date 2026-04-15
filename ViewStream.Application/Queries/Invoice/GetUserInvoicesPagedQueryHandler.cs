using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Invoice
{
    public class GetUserInvoicesPagedQueryHandler : IRequestHandler<GetUserInvoicesPagedQuery, PagedResult<InvoiceListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetUserInvoicesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PagedResult<InvoiceListItemDto>> Handle(GetUserInvoicesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Invoices.GetQueryable().Where(i => i.UserId == request.UserId);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(i => i.InvoiceDate)
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .AsNoTracking().ToListAsync(cancellationToken);
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
