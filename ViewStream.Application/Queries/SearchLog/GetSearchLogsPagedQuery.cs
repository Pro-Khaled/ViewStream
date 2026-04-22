using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.SearchLog
{
    public record GetSearchLogsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        long? ProfileId = null,
        string? Query = null
    ) : IRequest<PagedResult<SearchLogListItemDto>>;
}
