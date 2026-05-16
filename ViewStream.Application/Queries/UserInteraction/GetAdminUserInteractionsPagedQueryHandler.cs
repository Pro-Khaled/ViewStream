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

namespace ViewStream.Application.Queries.UserInteraction
{
    public class GetAdminUserInteractionsPagedQueryHandler : IRequestHandler<GetAdminUserInteractionsPagedQuery, PagedResult<AdminUserInteractionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminUserInteractionsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminUserInteractionListItemDto>> Handle(GetAdminUserInteractionsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserInteractions.GetQueryable()
                .AsNoTracking();

            if (request.ProfileId.HasValue)
                query = query.Where(s => s.ProfileId == request.ProfileId.Value);

            if (request.ShowId.HasValue)
                query = query.Where(s => s.ShowId == request.ShowId.Value);

            if (!string.IsNullOrWhiteSpace(request.InteractionType))
                query = query.Where(s => s.InteractionType == request.InteractionType);

            if (request.FromDate.HasValue)
                query = query.Where(s => s.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(s => s.CreatedAt <= request.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(s =>
                    s.InteractionType.Contains(term) ||
                    (s.Show != null && s.Show.Title.Contains(term)) ||
                    (s.Profile != null && s.Profile.Name.Contains(term)));
            }

            var projected = query.ProjectTo<AdminUserInteractionListItemDto>(_mapper.ConfigurationProvider);

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

            return new PagedResult<AdminUserInteractionListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
