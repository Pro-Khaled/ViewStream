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

namespace ViewStream.Application.Queries.ErrorLog
{
    public class GetErrorLogsPagedQueryHandler : IRequestHandler<GetErrorLogsPagedQuery, PagedResult<ErrorLogListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetErrorLogsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ErrorLogListItemDto>> Handle(GetErrorLogsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ErrorLogs.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.ErrorCode))
                query = query.Where(e => e.ErrorCode == request.ErrorCode);

            if (!string.IsNullOrWhiteSpace(request.Endpoint))
                query = query.Where(e => e.Endpoint != null && e.Endpoint.Contains(request.Endpoint));

            var totalCount = await query.CountAsync(cancellationToken);

            var logs = await query
                .OrderByDescending(e => e.OccurredAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<ErrorLogListItemDto>
            {
                Items = _mapper.Map<List<ErrorLogListItemDto>>(logs),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
