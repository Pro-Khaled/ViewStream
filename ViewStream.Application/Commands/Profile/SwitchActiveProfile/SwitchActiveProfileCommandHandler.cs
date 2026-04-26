using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;
using ViewStream.Shared.Options;

namespace ViewStream.Application.Commands.Profile.SwitchActiveProfile
{
    public class SwitchActiveProfileCommandHandler : IRequestHandler<SwitchActiveProfileCommand, SwitchProfileResponseDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtOptions _jwtOptions;

        public SwitchActiveProfileCommandHandler(IUnitOfWork unitOfWork, IOptions<JwtOptions> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<SwitchProfileResponseDto?> Handle(SwitchActiveProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync<long>(request.ProfileId, cancellationToken);
            if (profile == null || profile.UserId != request.UserId || profile.IsDeleted == true)
                return null;

            // Generate a new JWT with the ProfileId claim
            var user = await _unitOfWork.Users.GetByIdAsync<long>(request.UserId, cancellationToken);
            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(_jwtOptions.NameClaimType, user.FullName ?? user.Email!),
            new("ProfileId", profile.Id.ToString()),
            new("ProfileName", profile.Name),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new SwitchProfileResponseDto
            {
                ProfileId = profile.Id,
                ProfileName = profile.Name,
                Token = tokenString
            };
        }
    }
}
