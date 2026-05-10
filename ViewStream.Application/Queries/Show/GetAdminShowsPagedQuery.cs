using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Show
{
    public record GetAdminShowsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminShowListItemDto>>
    {
        public long? GenreId { get; init; }
        public int? ReleaseYear { get; init; }

        public GetAdminShowsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? genreId = null, int? releaseYear = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            GenreId = genreId;
            ReleaseYear = releaseYear;
        }
    }
}
