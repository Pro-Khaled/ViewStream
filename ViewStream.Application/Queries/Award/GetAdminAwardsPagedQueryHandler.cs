using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Award
{
    public class GetAdminAwardsPagedQueryHandler
        : IRequestHandler<GetAdminAwardsPagedQuery, PagedResult<AdminAwardListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminAwardsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminAwardListItemDto>> Handle(
            GetAdminAwardsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Awards.GetQueryable();




            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Name.Contains(request.SearchTerm));

            if (request.Year.HasValue)
                query = query.Where(s => s.Year == request.Year.Value);

            var projected = query.Select(s => new AdminAwardListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Category = s.Category,
                Year = s.Year,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {

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

            return new PagedResult<AdminAwardListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
