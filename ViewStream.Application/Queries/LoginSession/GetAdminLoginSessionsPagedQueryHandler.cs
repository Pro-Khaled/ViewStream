using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.LoginSession
{
    public class GetAdminLoginSessionsPagedQueryHandler : IRequestHandler<GetAdminLoginSessionsPagedQuery, PagedResult<AdminLoginSessionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminLoginSessionsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminLoginSessionListItemDto>> Handle(GetAdminLoginSessionsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.LoginSessions.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(ls => ls.UserId == request.UserId.Value);

            if (request.IsActive.HasValue)
            {
                var now = DateTime.UtcNow;
                if (request.IsActive.Value)
                    query = query.Where(ls => ls.RevokedAt == null && ls.ExpiresAt > now);
                else
                    query = query.Where(ls => ls.RevokedAt != null || ls.ExpiresAt <= now);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(ls => (ls.IpAddress != null && ls.IpAddress.Contains(request.SearchTerm)) || ls.SessionToken.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(ls => ls.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(ls => ls.CreatedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminLoginSessionListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(ls => ls.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Note: IsActive is likely calculated in DTO mapping or we can post-process if needed.
            // Assuming the ProjectTo handles the mapping logic defined in AutoMapper Profile.

            return new PagedResult<AdminLoginSessionListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
