using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserPromoUsage
{
    public record GetAdminUserPromoUsagesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminUserPromoUsageListItemDto>>
    {
        public long? UserId { get; init; }
        public int? PromoCodeId { get; init; }

        public GetAdminUserPromoUsagesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? userId = null, int? promoCodeId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
            PromoCodeId = promoCodeId;
        }
    }
}
