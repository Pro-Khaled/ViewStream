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

namespace ViewStream.Application.Queries.Show
{
    public class GetAdminShowsPagedQueryHandler : IRequestHandler<GetAdminShowsPagedQuery, PagedResult<AdminShowListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminShowsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminShowListItemDto>> Handle(GetAdminShowsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Shows.GetQueryable()
                .AsNoTracking();

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Title.Contains(request.SearchTerm));

            if (request.GenreId.HasValue)
                query = query.Where(s => s.Genres.Any(g => g.Id == request.GenreId.Value));
            
            if (request.ReleaseYear.HasValue)
                query = query.Where(s => s.ReleaseYear == request.ReleaseYear.Value);

            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.AddedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.AddedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(s => s.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(s => s.UpdatedAt <= request.UpdatedTo.Value);

            if (request.DeletedFrom.HasValue)
                query = query.Where(s => s.DeletedAt >= request.DeletedFrom.Value);
            if (request.DeletedTo.HasValue)
                query = query.Where(s => s.DeletedAt <= request.DeletedTo.Value);

            var projected = query.ProjectTo<AdminShowListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(s => s.AddedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminShowListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
