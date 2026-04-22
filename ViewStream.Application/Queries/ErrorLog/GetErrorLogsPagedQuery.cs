using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ErrorLog
{
    public record GetErrorLogsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? ErrorCode = null,
        string? Endpoint = null
    ) : IRequest<PagedResult<ErrorLogListItemDto>>;
}
