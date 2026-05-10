using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserInteraction
{
    public class GetAdminUserInteractionsPagedQueryHandler
        : IRequestHandler<GetAdminUserInteractionsPagedQuery, PagedResult<AdminUserInteractionListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminUserInteractionsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminUserInteractionListItemDto>> Handle(
            GetAdminUserInteractionsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.UserInteractions.GetQueryable();

            query = query.Include(e => e.Profile);             query = query.Include(e => e.Show);


            

            if (request.ProfileId.HasValue)
                query = query.Where(s => s.ProfileId == request.ProfileId.Value);
            if (request.ShowId.HasValue)
                query = query.Where(s => s.ShowId == request.ShowId.Value);
            if (!string.IsNullOrWhiteSpace(request.InteractionType))
                query = query.Where(s => s.InteractionType == request.InteractionType);
            if (request.FromDate.HasValue)
                query = query.Where(s => s.CreatedAt >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(s => s.CreatedAt <= request.ToDate.Value);

            var projected = query.Select(s => new AdminUserInteractionListItemDto
            {
                Id = s.Id,
                ProfileName = s.Profile.Name,
                ShowTitle = s.Show.Title,
                InteractionType = s.InteractionType,
                CreatedAt = s.CreatedAt,
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
                projected = projected.OrderByDescending(s => s.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminUserInteractionListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
