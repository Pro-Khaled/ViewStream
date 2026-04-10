using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.User.AdminUpdateUser
{
    using User = ViewStream.Domain.Entities.User;

    public class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public AdminUpdateUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(AdminUpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return false;

            if (request.Dto.FullName != null)
                user.FullName = request.Dto.FullName;
            if (request.Dto.PhoneNumber != null)
                user.PhoneNumber = request.Dto.PhoneNumber;
            if (request.Dto.IsActive.HasValue)
                user.IsActive = request.Dto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return false;

            if (request.Dto.Roles != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, request.Dto.Roles);
            }

            return true;
        }
    }
}
