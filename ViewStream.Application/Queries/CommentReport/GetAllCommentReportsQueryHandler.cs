using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentReport
{
    public class GetReportsPagedQueryHandler : IRequestHandler<GetReportsPagedQuery, PagedResult<CommentReportListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReportsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<CommentReportListItemDto>> Handle(GetReportsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.CommentReports.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(r => r.Status == request.Status);

            var totalCount = await query.CountAsync(cancellationToken);

            var reports = await query
                .OrderBy(r => r.Status == "pending" ? 0 : 1)
                .ThenByDescending(r => r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(r => r.Comment)
                .Include(r => r.ReportedByProfile)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<CommentReportListItemDto>
            {
                Items = _mapper.Map<List<CommentReportListItemDto>>(reports),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
