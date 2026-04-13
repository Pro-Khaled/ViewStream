using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentReport
{
    public class GetContentReportsPagedQueryHandler : IRequestHandler<GetContentReportsPagedQuery, PagedResult<ContentReportListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetContentReportsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ContentReportListItemDto>> Handle(GetContentReportsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ContentReports.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(r => r.Status == request.Status);

            if (!string.IsNullOrWhiteSpace(request.TargetType))
            {
                if (request.TargetType == "Show")
                    query = query.Where(r => r.ShowId != null);
                else if (request.TargetType == "Episode")
                    query = query.Where(r => r.EpisodeId != null);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var reports = await query
                .OrderBy(r => r.Status == "pending" ? 0 : 1)
                .ThenByDescending(r => r.ReportedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(r => r.Profile)
                .Include(r => r.Show)
                .Include(r => r.Episode)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<ContentReportListItemDto>
            {
                Items = _mapper.Map<List<ContentReportListItemDto>>(reports),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
