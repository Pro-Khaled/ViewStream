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

namespace ViewStream.Application.Queries.UserLibrary
{
    public class GetAdminUserLibrariesPagedQueryHandler : IRequestHandler<GetAdminUserLibrariesPagedQuery, PagedResult<AdminUserLibraryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminUserLibrariesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminUserLibraryListItemDto>> Handle(GetAdminUserLibrariesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserLibraries.GetQueryable()
                .AsNoTracking();

            if (request.ProfileId.HasValue)
                query = query.Where(ul => ul.ProfileId == request.ProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(ul => ul.Status == request.Status);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(ul => (ul.Show != null && ul.Show.Title.Contains(request.SearchTerm)) || ul.Profile.Name.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(ul => ul.AddedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(ul => ul.AddedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminUserLibraryListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(ul => ul.AddedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminUserLibraryListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
