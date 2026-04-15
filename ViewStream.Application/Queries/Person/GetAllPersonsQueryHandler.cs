using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Person
{
    public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, List<PersonListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPersonsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PersonListItemDto>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
        {
            var persons = await _unitOfWork.Persons.GetQueryable()
                .OrderBy(p => p.Name)
                .Include(p => p.Credits)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<PersonListItemDto>>(persons);
        }
    }

    public class GetPersonsPagedQueryHandler : IRequestHandler<GetPersonsPagedQuery, PagedResult<PersonListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPersonsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<PersonListItemDto>> Handle(GetPersonsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Persons.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(p => p.Name.Contains(request.SearchTerm));

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(p => p.Credits)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<PersonListItemDto>
            {
                Items = _mapper.Map<List<PersonListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }

}
