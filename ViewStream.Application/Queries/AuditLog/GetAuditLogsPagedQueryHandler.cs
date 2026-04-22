using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AuditLog
{
    public class GetAuditLogsPagedQueryHandler : IRequestHandler<GetAuditLogsPagedQuery, PagedResult<AuditLogListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAuditLogsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AuditLogListItemDto>> Handle(GetAuditLogsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.AuditLogs.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.TableName))
                query = query.Where(a => a.TableName == request.TableName);

            if (request.RecordId.HasValue)
                query = query.Where(a => a.RecordId == request.RecordId.Value);

            if (!string.IsNullOrWhiteSpace(request.Action))
                query = query.Where(a => a.Action == request.Action);

            if (request.ChangedByUserId.HasValue)
                query = query.Where(a => a.ChangedByUserId == request.ChangedByUserId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var logs = await query
                .OrderByDescending(a => a.ChangedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(a => a.ChangedByUser)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<AuditLogListItemDto>
            {
                Items = _mapper.Map<List<AuditLogListItemDto>>(logs),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
