using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var query = _unitOfWork.PromoCodes.GetQueryable();




            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Code.Contains(request.SearchTerm));

            if (request.IncludeExpired.HasValue)
                query = query.Where(s => request.IncludeExpired.Value || s.ValidUntil == null || s.ValidUntil >= DateOnly.FromDateTime(DateTime.UtcNow));

            var projected = query.Select(s => new AdminPromoCodeListItemDto
            {
                Id = s.Id,
                Code = s.Code,
                DiscountPercent = s.DiscountPercent,
                DiscountAmount = s.DiscountAmount,
                ValidUntil = s.ValidUntil,
                IsValid = s.ValidUntil == null || s.ValidUntil >= DateOnly.FromDateTime(DateTime.UtcNow),
                UsedCount = s.UsedCount ?? 0,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "isvalid" => desc ? projected.OrderByDescending(x => x.IsValid) : projected.OrderBy(x => x.IsValid),
                    "usedcount" => desc ? projected.OrderByDescending(x => x.UsedCount) : projected.OrderBy(x => x.UsedCount),
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
