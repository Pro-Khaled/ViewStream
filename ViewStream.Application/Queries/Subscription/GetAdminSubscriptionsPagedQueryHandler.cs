using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subscription
{
    public class GetAdminSubscriptionsPagedQueryHandler
        : IRequestHandler<GetAdminSubscriptionsPagedQuery, PagedResult<AdminSubscriptionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminSubscriptionsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminSubscriptionListItemDto>> Handle(
            GetAdminSubscriptionsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Subscriptions.GetQueryable();

            query = query.Include(e => e.User);


            

            if (request.UserId.HasValue)
                query = query.Where(s => s.UserId == request.UserId.Value);
if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);
if (!string.IsNullOrWhiteSpace(request.PlanType))
                query = query.Where(s => s.PlanType == request.PlanType);

            var projected = query.Select(s => new AdminSubscriptionListItemDto
            {
                Id = s.Id,
                UserId = s.UserId,
                UserEmail = s.User.Email,
                PlanType = s.PlanType,
                Status = s.Status,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                AutoRenew = s.AutoRenew,
                CreatedAt = s.CreatedAt,
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
                projected = projected.OrderByDescending(s => s.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminSubscriptionListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
