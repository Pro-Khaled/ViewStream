using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.API.Middleware
{
    /// <summary>
    /// Middleware that resolves the user's country from their IP address
    /// and stores it in HttpContext.Items["UserCountryCode"] for downstream access.
    /// </summary>
    public class GeoLocationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GeoLocationMiddleware> _logger;

        public GeoLocationMiddleware(RequestDelegate next, ILogger<GeoLocationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IGeoLocationService geoLocationService)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // Check for X-Forwarded-For header (reverse proxy scenarios)
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var firstIp = forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(firstIp))
                    ipAddress = firstIp;
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var countryCode = await geoLocationService.GetCountryCodeFromIpAsync(ipAddress);
                if (!string.IsNullOrEmpty(countryCode))
                {
                    context.Items["UserCountryCode"] = countryCode;
                    _logger.LogDebug("Resolved country {CountryCode} from IP {IP}", countryCode, ipAddress);
                }
            }

            await _next(context);
        }
    }
}
