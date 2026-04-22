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

namespace ViewStream.Application.Queries.SearchLog
{
    public class GetSearchLogsPagedQueryHandler : IRequestHandler<GetSearchLogsPagedQuery, PagedResult<SearchLogListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSearchLogsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<SearchLogListItemDto>> Handle(GetSearchLogsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SearchLogs.GetQueryable();

            if (request.ProfileId.HasValue)
                query = query.Where(s => s.ProfileId == request.ProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(s => s.Query.Contains(request.Query));

            var totalCount = await query.CountAsync(cancellationToken);

            var logs = await query
                .OrderByDescending(s => s.SearchAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(s => s.Profile)
                .Include(s => s.ClickedShow)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<SearchLogListItemDto>
            {
                Items = _mapper.Map<List<SearchLogListItemDto>>(logs),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
