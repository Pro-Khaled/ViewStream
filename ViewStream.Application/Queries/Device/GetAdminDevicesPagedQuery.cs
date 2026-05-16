using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Device
{
    public record GetAdminDevicesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminDeviceListItemDto>>
    {
        public long? UserId { get; init; }

        public GetAdminDevicesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? userId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
        }
    }
}
