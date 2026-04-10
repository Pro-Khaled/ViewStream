using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.User
{
    using User = ViewStream.Domain.Entities.User;


    public class GetUsersPagedQueryHandler : IRequestHandler<GetUsersPagedQuery, PagedResult<UserDto>>
    {
        private readonly UserManager<User> _userManager;

        public GetUsersPagedQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users
                .Where(u => !u.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(u =>
                    u.Email!.Contains(request.SearchTerm) ||
                    (u.FullName != null && u.FullName.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = new List<UserDto>();
            foreach (var user in users)
            {
                items.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    CountryCode = user.CountryCode,
                    IsActive = user.IsActive,
                    IsBlocked = user.IsBlocked,
                    BlockedReason = user.BlockedReason,
                    BlockedUntil = user.BlockedUntil,
                    CreatedAt = user.CreatedAt,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }

            return new PagedResult<UserDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
