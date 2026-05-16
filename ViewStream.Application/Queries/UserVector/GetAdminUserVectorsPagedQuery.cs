using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserVector
{
    public record GetAdminUserVectorsPagedQuery
        : AdminPagedQuery,
          IRequest<PagedResult<AdminUserVectorListItemDto>>
    {
        public GetAdminUserVectorsPagedQuery(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? sortBy = null,
            bool sortDescending = false,
            bool includeDeleted = false
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
        }
    }
}
