using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Features.Account.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IJwtTokenService jwtTokenService,
            IAuditContext auditContext,
            ILogger<LogoutCommandHandler> logger)
        {
            _jwtTokenService = jwtTokenService;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logout for UserId: {UserId}, token: {Token}...", request.UserId, request.RefreshToken[..10]);
            var success = await _jwtTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (success)
            {
                _auditContext.SetAudit<object, object>(
                    "RefreshTokens", 0, "REVOKE",
                    oldValues: null, newValues: new { request.RefreshToken }, changedByUserId: request.UserId
                );
            }

            return success;
        }
    }
}
