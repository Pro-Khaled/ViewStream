using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Genre
{
    public class GetGenresPagedQueryHandler : IRequestHandler<GetGenresPagedQuery, PagedResult<GenreListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetGenresPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<GenreListItemDto>> Handle(GetGenresPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Genres.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(g => g.Name.Contains(request.SearchTerm));

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(g => g.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(g => g.Shows)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<GenreListItemDto>
            {
                Items = _mapper.Map<List<GenreListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }


    public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, List<GenreListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllGenresQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GenreListItemDto>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
        {
            var genres = await _unitOfWork.Genres.GetQueryable()
                .OrderBy(g => g.Name)
                .Include(g => g.Shows)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<GenreListItemDto>>(genres);
        }
    }

}
