using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.AuditLog
{
    public record GetAuditLogsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? TableName = null,
        long? RecordId = null,
        string? Action = null,
        long? ChangedByUserId = null
    ) : IRequest<PagedResult<AuditLogListItemDto>>;
}
