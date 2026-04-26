using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs.Account;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Features.Account.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto?>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService, ILogger<RefreshTokenCommandHandler> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<AuthResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Token refresh request");
            return await _jwtTokenService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        }
    }
}
