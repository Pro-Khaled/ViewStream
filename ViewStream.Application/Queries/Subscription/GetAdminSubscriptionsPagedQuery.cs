using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Subscription
{
    public record GetAdminSubscriptionsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminSubscriptionListItemDto>>
    {
        public long? UserId { get; init; }
        public string? Status { get; init; }
        public string? PlanType { get; init; }

        public GetAdminSubscriptionsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? userId = null, string? status = null, string? planType = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
            Status = status;
            PlanType = planType;
        }
    }
}
