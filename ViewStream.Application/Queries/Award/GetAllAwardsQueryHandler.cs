using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Award
{

    public class GetAllAwardsQueryHandler : IRequestHandler<GetAllAwardsQuery, List<AwardListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllAwardsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<AwardListItemDto>> Handle(GetAllAwardsQuery request, CancellationToken cancellationToken)
        {
            var awards = await _unitOfWork.Awards.GetQueryable()
                .OrderByDescending(a => a.Year).ThenBy(a => a.Name)
                .AsNoTracking().ToListAsync(cancellationToken);
            return _mapper.Map<List<AwardListItemDto>>(awards);
        }
    }

    public class GetAwardsPagedQueryHandler : IRequestHandler<GetAwardsPagedQuery, PagedResult<AwardListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAwardsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PagedResult<AwardListItemDto>> Handle(GetAwardsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Awards.GetQueryable();
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(a => a.Name.Contains(request.SearchTerm) || (a.Category != null && a.Category.Contains(request.SearchTerm)));
            if (request.Year.HasValue)
                query = query.Where(a => a.Year == request.Year.Value);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(a => a.Year).ThenBy(a => a.Name)
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .AsNoTracking().ToListAsync(cancellationToken);
            return new PagedResult<AwardListItemDto>
            {
                Items = _mapper.Map<List<AwardListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
