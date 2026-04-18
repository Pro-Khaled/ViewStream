using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.UserRole.RemoveRoleFromUser
{
    using User = ViewStream.Domain.Entities.User;
    using Role = ViewStream.Domain.Entities.Role;
    public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public RemoveRoleFromUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager) { _userManager = userManager; _roleManager = roleManager; }
        public async Task<bool> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
            if (user == null || role == null) return false;
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return result.Succeeded;
        }
    }
}
