using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.CommentLike
{
    public record GetAdminCommentLikesPagedQuery : AdminPagedQuery, IRequest<PagedResult<CommentLikeListItemDto>>
    {
        public long? CommentId { get; init; }
        public long? ProfileId { get; init; }

        public GetAdminCommentLikesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? commentId = null, long? profileId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            CommentId = commentId;
            ProfileId = profileId;
        }
    }
}
