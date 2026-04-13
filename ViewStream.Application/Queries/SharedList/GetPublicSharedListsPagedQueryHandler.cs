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

namespace ViewStream.Application.Queries.SharedList
{
    public class GetPublicSharedListsPagedQueryHandler : IRequestHandler<GetPublicSharedListsPagedQuery, PagedResult<SharedListListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPublicSharedListsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<SharedListListItemDto>> Handle(GetPublicSharedListsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SharedLists.GetQueryable()
                .Where(l => l.IsPublic == true && l.IsDeleted != true);

            var totalCount = await query.CountAsync(cancellationToken);

            var lists = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(l => l.OwnerProfile)
                .Include(l => l.SharedListItems)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<SharedListListItemDto>
            {
                Items = _mapper.Map<List<SharedListListItemDto>>(lists),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
