using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using ViewStream.Application.Behaviors;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Shared.Options;

namespace ViewStream.API.Extensions;

public static class CacheExtensions
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Load RedisOptions
        var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();
        if (redisOptions == null)
            throw new InvalidOperationException("Redis options are not configured properly.");

        // 2. Register Redis distributed cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisOptions.ConnectionString;
            options.InstanceName = redisOptions.InstanceName;
        });

        // 3. Register IConnectionMultiplexer as a singleton (for advanced operations like invalidation)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisOptions.ConnectionString));

        // 4. Register caching behavior only if caching is enabled in settings
        var cacheOptions = configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>();
        if (cacheOptions?.Enabled == true)
        {
            // Order matters: CachingBehavior runs first (on read), then InvalidationBehavior runs after command handlers
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        }

        // Also register ICacheInvalidator (implementation must be added)
        services.AddScoped<ICacheInvalidator, RedisCacheInvalidator>();

        return services;
    }
}