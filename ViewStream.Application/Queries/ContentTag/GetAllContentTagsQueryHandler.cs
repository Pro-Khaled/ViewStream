using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentTag
{
    public class GetContentTagsPagedQueryHandler : IRequestHandler<GetContentTagsPagedQuery, PagedResult<ContentTagListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetContentTagsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<ContentTagListItemDto>> Handle(GetContentTagsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ContentTags.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(t => t.Name.Contains(request.SearchTerm) || (t.Category != null && t.Category.Contains(request.SearchTerm)));

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(t => t.Shows)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<ContentTagListItemDto>
            {
                Items = _mapper.Map<List<ContentTagListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }

    public class GetAllContentTagsQueryHandler : IRequestHandler<GetAllContentTagsQuery, List<ContentTagListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllContentTagsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ContentTagListItemDto>> Handle(GetAllContentTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _unitOfWork.ContentTags.GetQueryable()
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .Include(t => t.Shows)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ContentTagListItemDto>>(tags);
        }
    }
}
