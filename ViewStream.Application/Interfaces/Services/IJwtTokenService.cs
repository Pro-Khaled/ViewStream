using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs.Account;
using ViewStream.Domain.Entities;

namespace ViewStream.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a short‑lived JWT access token for the authenticated user.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <returns>JWT access token string.</returns>
        Task<string> GenerateAccessTokenAsync(User user);

        /// <summary>
        /// Generates a long‑lived refresh token and stores it in the database.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="jwtId">The unique identifier of the associated JWT.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The generated refresh token string.</returns>
        Task<string> GenerateRefreshTokenAsync(long userId, string jwtId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates a refresh token and issues a new access token / refresh token pair.
        /// </summary>
        /// <param name="refreshToken">The refresh token provided by the client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// An <see cref="AuthResponseDto"/> containing new tokens if successful; otherwise, <c>null</c>.
        /// </returns>
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a refresh token (e.g., during logout).
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><c>true</c> if the token was found and revoked; otherwise, <c>false</c>.</returns>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
