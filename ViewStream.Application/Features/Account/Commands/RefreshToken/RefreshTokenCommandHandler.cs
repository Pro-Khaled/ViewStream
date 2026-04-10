using MediatR;
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

        public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _jwtTokenService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        }
    }
}
