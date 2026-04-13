using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Country
{
    public class GetCountriesPagedQueryHandler : IRequestHandler<GetCountriesPagedQuery, PagedResult<CountryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCountriesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<CountryListItemDto>> Handle(GetCountriesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Countries.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(c => c.Name.Contains(request.SearchTerm) || c.Code.Contains(request.SearchTerm));

            if (!string.IsNullOrWhiteSpace(request.Continent))
                query = query.Where(c => c.Continent == request.Continent);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<CountryListItemDto>
            {
                Items = _mapper.Map<List<CountryListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }

    public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, List<CountryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCountriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CountryListItemDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.Countries.GetQueryable()
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CountryListItemDto>>(countries);
        }
    }
}
