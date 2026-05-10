using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Credit
{
    public record GetAdminCreditsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminCreditListItemDto>>
    {
        public string? Role { get; init; }

        public GetAdminCreditsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? role = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Role = role;
        }
    }
}
