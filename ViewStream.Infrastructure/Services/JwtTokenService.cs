using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ViewStream.Application.DTOs.Account;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;
using ViewStream.Shared.Options;

namespace ViewStream.Infrastructure.Services
{


    public class JwtTokenService : IJwtTokenService
    {

        private readonly JwtOptions _jwtOptions;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public JwtTokenService(
            IOptions<JwtOptions> jwtOptions,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        public async Task<string> GenerateAccessTokenAsync(User user)
{
    // 1. Validate configuration early
    if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
        throw new InvalidOperationException("JWT Key is not configured.");

    if (string.IsNullOrWhiteSpace(_jwtOptions.Algorithm))
        throw new InvalidOperationException("JWT Algorithm is not configured.");

    var tokenHandler = new JwtSecurityTokenHandler();
    
    // 2. Ensure key bytes are correctly generated
    var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);
    if (keyBytes.Length < 32) // HMAC-SHA256 requires at least 32 bytes (256 bits)
        throw new InvalidOperationException("JWT Key must be at least 32 characters long.");

    var securityKey = new SymmetricSecurityKey(keyBytes);

    // 3. Use the exact SecurityAlgorithms constant to avoid string mismatch
    var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email!),
        new(_jwtOptions.NameClaimType, user.FullName ?? user.Email!),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var roles = await _userManager.GetRolesAsync(user);
    claims.AddRange(roles.Select(role => new Claim(_jwtOptions.RoleClaimType, role)));

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
        Issuer = _jwtOptions.Issuer,
        Audience = _jwtOptions.Audience,
        SigningCredentials = signingCredentials
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
        public async Task<string> GenerateRefreshTokenAsync(long userId, string jwtId, CancellationToken cancellationToken = default)
        {
            // Generate cryptographically secure random token
            var tokenBytes = RandomNumberGenerator.GetBytes(_jwtOptions.RefreshTokenLength);
            var token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                JwtId = jwtId,
                IsUsed = false,
                IsRevoked = false,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays)
            };

            // Revoke existing tokens if multiple devices not allowed
            if (!_jwtOptions.AllowMultipleDevices)
            {
                var existingTokens = await _unitOfWork.RefreshTokens.FindAsync(
                    rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed,
                    asNoTracking: false,
                    cancellationToken: cancellationToken);

                foreach (var t in existingTokens)
                    t.IsRevoked = true;
            }

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return refreshToken.Token;
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Find token with User navigation eager loaded
            var tokens = await _unitOfWork.RefreshTokens.FindAsync(
                predicate: rt => rt.Token == refreshToken,
                include: q => q.Include(rt => rt.User),
                asNoTracking: false, // We'll update the token
                cancellationToken: cancellationToken);

            var storedToken = tokens.FirstOrDefault();

            if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
                return null;

            // Mark as used
            storedToken.IsUsed = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var user = storedToken.User;
            var newAccessToken = await GenerateAccessTokenAsync(user);
            var newJwtId = Guid.NewGuid().ToString();
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id, newJwtId, cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName ?? string.Empty,
                    Phone = user.PhoneNumber,
                    CountryCode = user.CountryCode,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                }
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var tokens = await _unitOfWork.RefreshTokens.FindAsync(
                rt => rt.Token == refreshToken,
                asNoTracking: false,
                cancellationToken: cancellationToken);

            var storedToken = tokens.FirstOrDefault();
            if (storedToken == null) return false;

            storedToken.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
