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

namespace ViewStream.Application.Queries.Rating
{
    public class GetAdminRatingsPagedQueryHandler : IRequestHandler<GetAdminRatingsPagedQuery, PagedResult<AdminRatingListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminRatingsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminRatingListItemDto>> Handle(GetAdminRatingsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Ratings.GetQueryable()
                .AsNoTracking();

            if (request.ShowId.HasValue)
                query = query.Where(r => r.ShowId == request.ShowId.Value);

            if (request.ProfileId.HasValue)
                query = query.Where(r => r.ProfileId == request.ProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(r => r.Show.Title.Contains(request.SearchTerm) || r.Profile.Name.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(r => r.RatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(r => r.RatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(r => r.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(r => r.UpdatedAt <= request.UpdatedTo.Value);

            var projected = query.ProjectTo<AdminRatingListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(r => r.RatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminRatingListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
