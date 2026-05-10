using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PromoCode
{
    public record GetAdminPromoCodesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminPromoCodeListItemDto>>
    {
        public bool? IncludeExpired { get; init; }

        public GetAdminPromoCodesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, bool? includeExpired = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            IncludeExpired = includeExpired;
        }
    }
}
