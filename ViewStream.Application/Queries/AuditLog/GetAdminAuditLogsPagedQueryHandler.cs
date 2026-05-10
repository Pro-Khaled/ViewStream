using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AuditLog
{
    public class GetAdminAuditLogsPagedQueryHandler
        : IRequestHandler<GetAdminAuditLogsPagedQuery, PagedResult<AdminAuditLogListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminAuditLogsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminAuditLogListItemDto>> Handle(
            GetAdminAuditLogsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.AuditLogs.GetQueryable();

            query = query.Include(e => e.ChangedByUser);


            

            if (!string.IsNullOrWhiteSpace(request.TableName))
                query = query.Where(s => s.TableName == request.TableName);
            if (request.RecordId.HasValue)
                query = query.Where(s => s.RecordId == request.RecordId.Value);
            if (!string.IsNullOrWhiteSpace(request.Action))
                query = query.Where(s => s.Action == request.Action);
            if (request.ChangedByUserId.HasValue)
                query = query.Where(s => s.ChangedByUserId == request.ChangedByUserId.Value);

            var projected = query.Select(s => new AdminAuditLogListItemDto
            {
                Id = s.Id,
                TableName = s.TableName,
                RecordId = s.RecordId,
                Action = s.Action,
                ChangedByUserName = s.ChangedByUser != null ? s.ChangedByUser.UserName : null,
                ChangedAt = s.ChangedAt,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "changedbyusername" => desc ? projected.OrderByDescending(x => x.ChangedByUserName) : projected.OrderBy(x => x.ChangedByUserName),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.ChangedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminAuditLogListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
