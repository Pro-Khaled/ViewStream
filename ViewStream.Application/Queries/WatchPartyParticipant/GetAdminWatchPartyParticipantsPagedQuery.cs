using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.WatchPartyParticipant
{
    public record GetAdminWatchPartyParticipantsPagedQuery : AdminPagedQuery, IRequest<PagedResult<WatchPartyParticipantDto>>
    {
        public long? WatchPartyId { get; init; }

        public GetAdminWatchPartyParticipantsPagedQuery(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? sortBy = null,
            bool sortDescending = false,
            bool includeDeleted = false,
            long? watchPartyId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            WatchPartyId = watchPartyId;
        }
    }
}
