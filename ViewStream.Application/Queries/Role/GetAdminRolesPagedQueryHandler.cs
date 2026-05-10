using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Role
{
    public class GetAdminRolesPagedQueryHandler
        : IRequestHandler<GetAdminRolesPagedQuery, PagedResult<AdminRoleListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminRolesPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminRoleListItemDto>> Handle(
            GetAdminRolesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Roles.GetQueryable();




            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Name.Contains(request.SearchTerm));

            

            var projected = query.Select(s => new AdminRoleListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                IsSystem = s.IsSystem,
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
                projected = projected.OrderBy(s => s.Name);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminRoleListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
