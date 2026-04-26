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

namespace ViewStream.Application.Queries.DataDeletionRequest
{
    public class GetDataDeletionRequestsPagedQueryHandler : IRequestHandler<GetDataDeletionRequestsPagedQuery, PagedResult<DataDeletionRequestListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDataDeletionRequestsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<DataDeletionRequestListItemDto>> Handle(GetDataDeletionRequestsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.DataDeletionRequests.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(r => r.Status == request.Status);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(r => r.RequestedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(r => r.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<DataDeletionRequestListItemDto>
            {
                Items = _mapper.Map<List<DataDeletionRequestListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
