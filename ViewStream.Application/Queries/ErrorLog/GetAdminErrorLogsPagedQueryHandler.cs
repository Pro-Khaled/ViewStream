using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ErrorLog
{
    public class GetAdminErrorLogsPagedQueryHandler
        : IRequestHandler<GetAdminErrorLogsPagedQuery, PagedResult<AdminErrorLogListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminErrorLogsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminErrorLogListItemDto>> Handle(
            GetAdminErrorLogsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ErrorLogs.GetQueryable();

            query = query.Include(e => e.User);


            

            if (!string.IsNullOrWhiteSpace(request.ErrorCode))
                query = query.Where(s => s.ErrorCode == request.ErrorCode);
if (!string.IsNullOrWhiteSpace(request.Endpoint))
                query = query.Where(s => s.Endpoint == request.Endpoint);

            var projected = query.Select(s => new AdminErrorLogListItemDto
            {
                Id = s.Id,
                ErrorCode = s.ErrorCode,
                ErrorMessage = s.ErrorMessage,
                Endpoint = s.Endpoint,
                OccurredAt = s.OccurredAt,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {

                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.OccurredAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminErrorLogListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
