using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Show
{
    public record GetShowsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? SearchTerm = null,
        long? GenreId = null,
        int? ReleaseYear = null,
        bool IncludeDeleted = false
    ) : IRequest<PagedResult<ShowListItemDto>>;
}
