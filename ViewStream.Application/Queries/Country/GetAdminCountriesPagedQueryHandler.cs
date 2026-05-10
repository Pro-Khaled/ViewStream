using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Country
{
    public class GetAdminCountriesPagedQueryHandler
        : IRequestHandler<GetAdminCountriesPagedQuery, PagedResult<AdminCountryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminCountriesPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminCountryListItemDto>> Handle(
            GetAdminCountriesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Countries.GetQueryable();

            query = query.Include(e => e.ShowAvailabilities);


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Name.Contains(request.SearchTerm) || s.Code.Contains(request.SearchTerm));

            if (!string.IsNullOrWhiteSpace(request.Continent))
                query = query.Where(s => s.Continent == request.Continent);

            var projected = query.Select(s => new AdminCountryListItemDto
            {
                Code = s.Code,
                Name = s.Name,
                Continent = s.Continent,
                AvailabilityCount = s.ShowAvailabilities.Count,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "availabilitycount" => desc ? projected.OrderByDescending(x => x.AvailabilityCount) : projected.OrderBy(x => x.AvailabilityCount),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderBy(s => s.Name);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminCountryListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
