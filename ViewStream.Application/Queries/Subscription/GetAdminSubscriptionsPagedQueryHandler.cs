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

namespace ViewStream.Application.Queries.Subscription
{
    public class GetAdminSubscriptionsPagedQueryHandler : IRequestHandler<GetAdminSubscriptionsPagedQuery, PagedResult<AdminSubscriptionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminSubscriptionsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminSubscriptionListItemDto>> Handle(GetAdminSubscriptionsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Subscriptions.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(s => s.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);

            if (!string.IsNullOrWhiteSpace(request.PlanType))
                query = query.Where(s => s.PlanType == request.PlanType);

            var projected = query.ProjectTo<AdminSubscriptionListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(s => s.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminSubscriptionListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
