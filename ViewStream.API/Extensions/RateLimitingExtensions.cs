using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Shared.Options;

namespace ViewStream.API.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
                          ?? new RateLimitingOptions();

            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                rateLimiterOptions.AddFixedWindowLimiter("AuthRateLimit", cfg =>
                {
                    cfg.PermitLimit = options.AuthPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.AuthWindowSeconds);
                    cfg.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    cfg.QueueLimit = 0;
                });

                rateLimiterOptions.AddFixedWindowLimiter("SearchRateLimit", cfg =>
                {
                    cfg.PermitLimit = options.SearchPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.SearchWindowSeconds);
                });
            });

            return services;
        }
    }
}