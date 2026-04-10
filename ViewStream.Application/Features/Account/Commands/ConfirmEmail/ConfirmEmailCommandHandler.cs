using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Entities;

namespace ViewStream.Application.Features.Account.Commands.ConfirmEmail
{

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResult>
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return new ConfirmEmailResult(false, "User not found.");

            if (user.EmailConfirmed)
                return new ConfirmEmailResult(true); // Already confirmed – success

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
                return new ConfirmEmailResult(false, "Invalid or expired confirmation token.");

            return new ConfirmEmailResult(true);
        }
    }
}
