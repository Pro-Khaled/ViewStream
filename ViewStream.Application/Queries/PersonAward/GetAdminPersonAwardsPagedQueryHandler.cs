using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PersonAward
{
    public class GetAdminPersonAwardsPagedQueryHandler : IRequestHandler<GetAdminPersonAwardsPagedQuery, PagedResult<AdminPersonAwardListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminPersonAwardsPagedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<AdminPersonAwardListItemDto>> Handle(GetAdminPersonAwardsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PersonAwards.GetQueryable()
                .Include(pa => pa.Person)
                .Include(pa => pa.Award)
                .AsQueryable();

            if (request.PersonId.HasValue)
                query = query.Where(pa => pa.PersonId == request.PersonId.Value);

            if (request.AwardId.HasValue)
                query = query.Where(pa => pa.AwardId == request.AwardId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(pa => pa.Person.Name.Contains(request.SearchTerm) || pa.Award.Name.Contains(request.SearchTerm));

            var projected = query.Select(pa => new AdminPersonAwardListItemDto
            {
                PersonId = pa.PersonId,
                PersonName = pa.Person.Name,
                AwardId = pa.AwardId,
                AwardName = pa.Award.Name,
                AwardCategory = pa.Award.Category,
                AwardYear = pa.Award.Year,
                Won = pa.Won
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                // Default sort: Year descending
                projected = projected.OrderByDescending(pa => pa.AwardYear).ThenBy(pa => pa.AwardName);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminPersonAwardListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
