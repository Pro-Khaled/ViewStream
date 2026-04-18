using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserRole
{
    using User = ViewStream.Domain.Entities.User;
    using Role = ViewStream.Domain.Entities.Role;
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<UserRoleDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public GetUserRolesQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager) { _userManager = userManager; _roleManager = roleManager; }
        public async Task<List<UserRoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return new List<UserRoleDto>();
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync(cancellationToken);
            return roles.Select(r => new UserRoleDto { UserId = user.Id, UserEmail = user.Email, RoleId = r.Id, RoleName = r.Name }).ToList();
        }
    }
}
