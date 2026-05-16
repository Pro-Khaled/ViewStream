using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PaymentMethod
{
    public record GetAdminPaymentMethodsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminPaymentMethodListItemDto>>
    {
        public long? UserId { get; init; }

        public GetAdminPaymentMethodsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? userId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
        }
    }
}
