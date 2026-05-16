using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Friendship
{
    public record GetAdminFriendshipsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminFriendshipListItemDto>>
    {
        public long? UserId { get; init; }
        public long? FriendId { get; init; }
        public string? Status { get; init; }

        public GetAdminFriendshipsPagedQuery(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? sortBy = null,
            bool sortDescending = false,
            bool includeDeleted = false,
            long? userId = null,
            long? friendId = null,
            string? status = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            UserId = userId;
            FriendId = friendId;
            Status = status;
        }
    }
}
