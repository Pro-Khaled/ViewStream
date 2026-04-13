using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserLibrary
{
    public class GetUserLibraryPagedQueryHandler : IRequestHandler<GetUserLibraryPagedQuery, PagedResult<UserLibraryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserLibraryPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserLibraryListItemDto>> Handle(GetUserLibraryPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserLibraries.GetQueryable()
                .Where(ul => ul.ProfileId == request.ProfileId);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(ul => ul.Status == request.Status);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(ul => ul.AddedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(ul => ul.Show)
                .Include(ul => ul.Season).ThenInclude(s => s.Show)
                .Include(ul => ul.Season).ThenInclude(s => s.Episodes)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtos = items.Select(item =>
            {
                var dto = _mapper.Map<UserLibraryListItemDto>(item);
                if (item.Season != null)
                {
                    dto.TotalEpisodes = item.Season.Episodes.Count(e => e.IsDeleted != true);
                    dto.ItemType = "Season";
                }
                else if (item.Show != null)
                {
                    dto.TotalEpisodes = item.Show.Seasons
                        .Where(s => s.IsDeleted != true)
                        .SelectMany(s => s.Episodes)
                        .Count(e => e.IsDeleted != true);
                }
                return dto;
            }).ToList();

            return new PagedResult<UserLibraryListItemDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
