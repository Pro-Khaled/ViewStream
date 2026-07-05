using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;

namespace ViewStream.API.Middleware
{
    /// <summary>
    /// Blocks API access for users with pending or approved data deletion requests.
    /// Checks only authenticated requests.
    /// </summary>
    public class DataDeletionBlockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DataDeletionBlockMiddleware> _logger;

        public DataDeletionBlockMiddleware(RequestDelegate next, ILogger<DataDeletionBlockMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst("uid")?.Value
                    ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (long.TryParse(userIdClaim, out var userId))
                {
                    var deletionRequests = await unitOfWork.DataDeletionRequests.FindAsync(
                        r => r.UserId == userId && (r.Status == "pending" || r.Status == "approved"),
                        asNoTracking: true);

                    if (deletionRequests.Any())
                    {
                        _logger.LogWarning("Blocked request from User {UserId} with pending/approved data deletion.", userId);
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Error = "Your account is scheduled for deletion. API access is blocked.",
                            Status = deletionRequests.First().Status
                        });
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
