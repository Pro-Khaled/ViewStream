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

namespace ViewStream.Application.Queries.ShowAvailability
{
    public class GetAdminShowAvailabilitiesPagedQueryHandler : IRequestHandler<GetAdminShowAvailabilitiesPagedQuery, PagedResult<AdminShowAvailabilityListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminShowAvailabilitiesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminShowAvailabilityListItemDto>> Handle(GetAdminShowAvailabilitiesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ShowAvailabilities.GetQueryable()
                .AsNoTracking();

            query = query.Include(sa => sa.CountryCodeNavigation);

            if (request.ShowId.HasValue)
                query = query.Where(sa => sa.ShowId == request.ShowId.Value);

            if (!string.IsNullOrWhiteSpace(request.CountryCode))
                query = query.Where(sa => sa.CountryCode == request.CountryCode);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(sa =>
                    sa.Show.Title.Contains(request.SearchTerm) ||
                    sa.CountryCodeNavigation.Name.Contains(request.SearchTerm));

            if (request.UpdatedFrom.HasValue)
                query = query.Where(sa => sa.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(sa => sa.UpdatedAt <= request.UpdatedTo.Value);

            // AvailableFrom / AvailableUntil are business date fields, not audit fields,
            // so they are intentionally NOT filtered by the AdminPagedQuery date range params.
                
            var projected = query.ProjectTo<AdminShowAvailabilityListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderBy(sa => sa.ShowId).ThenBy(sa => sa.CountryCode);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminShowAvailabilityListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
