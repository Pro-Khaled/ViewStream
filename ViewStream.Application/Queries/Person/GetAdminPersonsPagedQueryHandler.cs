using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Person
{
    public class GetAdminPersonsPagedQueryHandler
        : IRequestHandler<GetAdminPersonsPagedQuery, PagedResult<AdminPersonListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminPersonsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminPersonListItemDto>> Handle(
            GetAdminPersonsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Persons.GetQueryable()
                .AsNoTracking();

            // Soft-delete filter
            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            // Date-range filters
            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(s => s.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(s => s.UpdatedAt <= request.UpdatedTo.Value);

            // Text search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Name.Contains(request.SearchTerm));

            var projected = query.Select(s => new AdminPersonListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Bio = s.Bio,
                PhotoUrl = s.PhotoUrl,
                CreditCount = s.Credits.Count,
                AwardCount = s.PersonAwards.Count,
                BirthDate = s.BirthDate,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                IsDeleted = s.IsDeleted
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "creditcount" => desc ? projected.OrderByDescending(x => x.CreditCount) : projected.OrderBy(x => x.CreditCount),
                    "awardcount" => desc ? projected.OrderByDescending(x => x.AwardCount) : projected.OrderBy(x => x.AwardCount),
                    "createdat" => desc ? projected.OrderByDescending(x => x.CreatedAt) : projected.OrderBy(x => x.CreatedAt),
                    "updatedat" => desc ? projected.OrderByDescending(x => x.UpdatedAt) : projected.OrderBy(x => x.UpdatedAt),
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

            return new PagedResult<AdminPersonListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
