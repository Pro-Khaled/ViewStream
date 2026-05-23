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

namespace ViewStream.Application.Queries.SharedList
{
    public class GetAdminSharedListsPagedQueryHandler : IRequestHandler<GetAdminSharedListsPagedQuery, PagedResult<AdminSharedListListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminSharedListsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminSharedListListItemDto>> Handle(GetAdminSharedListsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SharedLists.GetQueryable()
                .AsNoTracking();

            if (!request.IncludeDeleted)
                query = query.Where(sl => sl.IsDeleted != true);

            if (request.OwnerProfileId.HasValue)
                query = query.Where(sl => sl.OwnerProfileId == request.OwnerProfileId.Value);

            if (request.IsPublic.HasValue)
                query = query.Where(sl => sl.IsPublic == request.IsPublic.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(sl => sl.Name.Contains(request.SearchTerm) || sl.OwnerProfile.Name.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(sl => sl.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(sl => sl.CreatedAt <= request.CreatedTo.Value);

            if (request.DeletedFrom.HasValue)
                query = query.Where(sl => sl.DeletedAt >= request.DeletedFrom.Value);
            if (request.DeletedTo.HasValue)
                query = query.Where(sl => sl.DeletedAt <= request.DeletedTo.Value);

            var projected = query.ProjectTo<AdminSharedListListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(sl => sl.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminSharedListListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
