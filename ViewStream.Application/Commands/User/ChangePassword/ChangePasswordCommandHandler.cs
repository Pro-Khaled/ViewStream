using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.User.ChangePassword
{
    using User = ViewStream.Domain.Entities.User;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public ChangePasswordCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ChangePasswordAsync(user,
                request.Dto.CurrentPassword,
                request.Dto.NewPassword);
        }
    }
}
