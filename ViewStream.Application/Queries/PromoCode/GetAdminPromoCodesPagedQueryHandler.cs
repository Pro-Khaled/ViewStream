using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PromoCode
{
    public class GetAdminPromoCodesPagedQueryHandler
        : IRequestHandler<GetAdminPromoCodesPagedQuery, PagedResult<AdminPromoCodeListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminPromoCodesPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminPromoCodeListItemDto>> Handle(
            GetAdminPromoCodesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PromoCodes.GetQueryable()
                .AsNoTracking();

            // Text search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Code.Contains(request.SearchTerm));

            // IncludeExpired filter (default include all unless explicitly false)
            if (request.IncludeExpired.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                query = query.Where(s => request.IncludeExpired.Value || s.ValidUntil == null || s.ValidUntil >= today);
            }

            // Date-range filters
            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(s => s.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(s => s.UpdatedAt <= request.UpdatedTo.Value);

            var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);

            var projected = query.Select(s => new AdminPromoCodeListItemDto
            {
                Id = s.Id,
                Code = s.Code,
                DiscountPercent = s.DiscountPercent,
                DiscountAmount = s.DiscountAmount,
                ValidFrom = s.ValidFrom,
                ValidUntil = s.ValidUntil,
                IsValid = s.ValidFrom <= todayDate && 
                          (s.ValidUntil == null || s.ValidUntil >= todayDate) && 
                          (s.MaxUses == null || s.UsedCount < s.MaxUses),
                UsedCount = s.UsedCount ?? 0,
                MaxUses = s.MaxUses,
                AppliesToPlan = s.AppliesToPlan,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "isvalid" => desc ? projected.OrderByDescending(x => x.IsValid) : projected.OrderBy(x => x.IsValid),
                    "usedcount" => desc ? projected.OrderByDescending(x => x.UsedCount) : projected.OrderBy(x => x.UsedCount),
                    "createdat" => desc ? projected.OrderByDescending(x => x.CreatedAt) : projected.OrderBy(x => x.CreatedAt),
                    "updatedat" => desc ? projected.OrderByDescending(x => x.UpdatedAt) : projected.OrderBy(x => x.UpdatedAt),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderBy(s => s.Id);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminPromoCodeListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
