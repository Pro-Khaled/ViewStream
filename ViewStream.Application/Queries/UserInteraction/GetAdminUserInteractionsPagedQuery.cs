using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserInteraction
{
    public record GetAdminUserInteractionsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminUserInteractionListItemDto>>
    {
        public long? ProfileId { get; init; }
        public long? ShowId { get; init; }
        public string? InteractionType { get; init; }
        public DateTime? FromDate { get; init; }
        public DateTime? ToDate { get; init; }

        public GetAdminUserInteractionsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? profileId = null, long? showId = null, string? interactionType = null, DateTime? fromDate = null, DateTime? toDate = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ProfileId = profileId;
            ShowId = showId;
            InteractionType = interactionType;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
