using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.OfflineDownload
{
    public record GetAdminOfflineDownloadsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminOfflineDownloadListItemDto>>
    {
        public long? ProfileId { get; init; }

        public GetAdminOfflineDownloadsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? profileId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ProfileId = profileId;
        }
    }
}
