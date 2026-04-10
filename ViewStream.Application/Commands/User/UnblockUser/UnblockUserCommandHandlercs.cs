using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.User.UnblockUser
{
    using User = ViewStream.Domain.Entities.User;

    public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public UnblockUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return false;

            user.IsBlocked = false;
            user.BlockedReason = null;
            user.BlockedUntil = null;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
