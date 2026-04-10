using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Features.Account.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IJwtTokenService _jwtTokenService;

        public LogoutCommandHandler(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _jwtTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        }
    }
}
