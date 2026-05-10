using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.AuditLog
{
    public record GetAdminAuditLogsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminAuditLogListItemDto>>
    {
        public string? TableName { get; init; }
        public long? RecordId { get; init; }
        public string? Action { get; init; }
        public long? ChangedByUserId { get; init; }

        public GetAdminAuditLogsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? tableName = null, long? recordId = null, string? action = null, long? changedByUserId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            TableName = tableName;
            RecordId = recordId;
            Action = action;
            ChangedByUserId = changedByUserId;
        }
    }
}
