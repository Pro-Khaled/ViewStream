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

namespace ViewStream.Application.Queries.ShowAward
{
    public class GetAdminShowAwardsPagedQueryHandler : IRequestHandler<GetAdminShowAwardsPagedQuery, PagedResult<AdminShowAwardListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminShowAwardsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminShowAwardListItemDto>> Handle(GetAdminShowAwardsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ShowAwards.GetQueryable()
                .AsNoTracking();

            if (request.ShowId.HasValue)
                query = query.Where(sa => sa.ShowId == request.ShowId.Value);

            if (request.AwardId.HasValue)
                query = query.Where(sa => sa.AwardId == request.AwardId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(sa => sa.Show.Title.Contains(request.SearchTerm) || sa.Award.Name.Contains(request.SearchTerm));

            var projected = query.ProjectTo<AdminShowAwardListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(sa => sa.AwardYear).ThenBy(sa => sa.AwardName);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminShowAwardListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
