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

namespace ViewStream.Application.Queries.Show
{
    public class GetShowsPagedQueryHandler : IRequestHandler<GetShowsPagedQuery, PagedResult<ShowListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetShowsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ShowListItemDto>> Handle(GetShowsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Shows.GetQueryable();

            // Filtering
            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Title.Contains(request.SearchTerm));

            if (request.GenreId.HasValue)
                query = query.Where(s => s.Genres.Any(g => g.Id == request.GenreId.Value));

            if (request.ReleaseYear.HasValue)
                query = query.Where(s => s.ReleaseYear == request.ReleaseYear.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var shows = await query
                .OrderByDescending(s => s.AddedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(s => s.Genres)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<ShowListItemDto>
            {
                Items = _mapper.Map<List<ShowListItemDto>>(shows),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
