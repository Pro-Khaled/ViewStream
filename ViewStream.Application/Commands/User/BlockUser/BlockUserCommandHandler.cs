using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.User.BlockUser
{
    using User = ViewStream.Domain.Entities.User;

    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public BlockUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return false;

            user.IsBlocked = true;
            user.BlockedReason = request.Dto.Reason;
            user.BlockedUntil = request.Dto.BlockedUntil;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
