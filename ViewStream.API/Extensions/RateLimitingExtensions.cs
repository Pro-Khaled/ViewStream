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

                rateLimiterOptions.AddFixedWindowLimiter("PublicReadRateLimit", cfg => {
                    cfg.PermitLimit = options.PublicReadPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.PublicReadWindowSeconds);
                });

                rateLimiterOptions.AddFixedWindowLimiter("UserWriteRateLimit", cfg => {
                    cfg.PermitLimit = options.UserWritePermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.UserWriteWindowSeconds);
                });

                rateLimiterOptions.AddFixedWindowLimiter("ReportRateLimit", cfg => {
                    cfg.PermitLimit = options.ReportPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.ReportWindowSeconds);
                });

                rateLimiterOptions.AddFixedWindowLimiter("AdminRateLimit", cfg => {
                    cfg.PermitLimit = options.AdminPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.AdminWindowSeconds);
                });

                rateLimiterOptions.AddFixedWindowLimiter("AnalyticsRateLimit", cfg => {
                    cfg.PermitLimit = options.AnalyticsPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.AnalyticsWindowSeconds);
                });

                rateLimiterOptions.AddFixedWindowLimiter("ContentManagementRateLimit", cfg => {
                    cfg.PermitLimit = options.ContentManagementPermitLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.ContentManagementWindowSeconds);
                });

                rateLimiterOptions.AddFixedWindowLimiter("DefaultRateLimit", cfg => {
                    cfg.PermitLimit = options.DefaultRateLimit;
                    cfg.Window = TimeSpan.FromSeconds(options.DefaultRateLimitWindowSeconds);
                });

            });

            return services;
        }
    }
}