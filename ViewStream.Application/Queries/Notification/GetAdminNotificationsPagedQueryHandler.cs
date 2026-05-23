using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Notification
{
    public class GetAdminNotificationsPagedQueryHandler : IRequestHandler<GetAdminNotificationsPagedQuery, PagedResult<AdminNotificationListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminNotificationsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminNotificationListItemDto>> Handle(GetAdminNotificationsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Notifications.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(n => n.UserId == request.UserId.Value);

            if (request.IsRead.HasValue)
                query = query.Where(n => n.IsRead == request.IsRead.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(n => (n.Title != null && n.Title.Contains(request.SearchTerm)) || (n.User != null && n.User.Email.Contains(request.SearchTerm)));

            if (request.CreatedFrom.HasValue)
                query = query.Where(n => n.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(n => n.CreatedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminNotificationListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(n => n.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminNotificationListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
