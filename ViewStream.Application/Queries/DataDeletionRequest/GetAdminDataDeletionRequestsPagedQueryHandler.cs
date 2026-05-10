using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.DataDeletionRequest
{
    public class GetAdminDataDeletionRequestsPagedQueryHandler
        : IRequestHandler<GetAdminDataDeletionRequestsPagedQuery, PagedResult<AdminDataDeletionRequestListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminDataDeletionRequestsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminDataDeletionRequestListItemDto>> Handle(
            GetAdminDataDeletionRequestsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.DataDeletionRequests.GetQueryable();

            query = query.Include(e => e.User);


            

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);

            var projected = query.Select(s => new AdminDataDeletionRequestListItemDto
            {
                Id = s.Id,
                UserEmail = s.User.Email,
                Status = s.Status,
                RequestedAt = s.RequestedAt,
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
                projected = projected.OrderByDescending(s => s.RequestedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminDataDeletionRequestListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
