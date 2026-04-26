using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;

namespace ViewStream.Application.Features.Account.Commands.ConfirmEmail
{

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<ConfirmEmailCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<ConfirmEmailResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return new ConfirmEmailResult(false, "User not found.");

            if (user.EmailConfirmed)
                return new ConfirmEmailResult(true);

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
                return new ConfirmEmailResult(false, "Invalid or expired confirmation token.");

            _auditContext.SetAudit<User, object>(
                "Users", user.Id, "UPDATE",
                oldValues: new { EmailConfirmed = false },
                newValues: new { EmailConfirmed = true },
                changedByUserId: user.Id
            );

            _logger.LogInformation("Email confirmed for UserId: {UserId}", user.Id);
            return new ConfirmEmailResult(true);
        }
    }
}
