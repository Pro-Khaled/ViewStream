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

namespace ViewStream.Application.Queries.UserPromoUsage
{
    public class GetAdminUserPromoUsagesPagedQueryHandler : IRequestHandler<GetAdminUserPromoUsagesPagedQuery, PagedResult<AdminUserPromoUsageListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminUserPromoUsagesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminUserPromoUsageListItemDto>> Handle(GetAdminUserPromoUsagesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserPromoUsages.GetQueryable()
                .AsNoTracking();

            if (request.UserId.HasValue)
                query = query.Where(upu => upu.UserId == request.UserId.Value);

            if (request.PromoCodeId.HasValue)
                query = query.Where(upu => upu.PromoCodeId == request.PromoCodeId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(upu => upu.User.Email.Contains(request.SearchTerm) || upu.PromoCode.Code.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(upu => upu.UsedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(upu => upu.UsedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminUserPromoUsageListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(upu => upu.UsedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminUserPromoUsageListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
