using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ViewStream.Application.Common;
using ViewStream.Shared.Options;

namespace ViewStream.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that caches responses for queries implementing <see cref="ICacheableQuery"/>.
/// </summary>
/// <typeparam name="TRequest">Request type (must implement <see cref="IRequest{TResponse}"/>).</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        IDistributedCache cache,
        CacheOptions cacheOptions,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //// Check if the request type (or its base types) has the [Cacheable] attribute
        //var isCacheable = typeof(TRequest).GetCustomAttribute<CacheableAttribute>(inherit: true) != null;
        //if (!isCacheable)
        //{
        //    _logger.LogTrace("Request {RequestType} is not marked [Cacheable] – skipping cache.", typeof(TRequest).Name);
        //    return await next();
        //}

        // Determine if this query should be cached based on configuration
        var queryName = typeof(TRequest).Name;
        var isCacheable = _cacheOptions.CacheableQueries
            .Any(q => string.Equals(q.Name, queryName, StringComparison.OrdinalIgnoreCase));

        if (!isCacheable)
        {
            _logger.LogTrace("Request {RequestType} is not configured as cacheable – skipping cache.", typeof(TRequest).Name);
            return await next();
        }

        // Build a deterministic cache key based on the query type and its serialized content
        var cacheKey = GenerateCacheKey(request);
        _logger.LogDebug("Checking cache for {RequestType} with key: {CacheKey}", typeof(TRequest).Name, cacheKey);

        // Try to get cached value
        var cachedBytes = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedBytes != null)
        {
            _logger.LogDebug("Cache HIT for {RequestType}", typeof(TRequest).Name);
            var cachedString = Encoding.UTF8.GetString(cachedBytes);
            return JsonSerializer.Deserialize<TResponse>(cachedString)!;
        }

        _logger.LogDebug("Cache MISS for {RequestType} – executing handler.", typeof(TRequest).Name);
        var response = await next();

        // Store response in cache with the appropriate TTL
        var ttlSeconds = GetTtlForQuery<TRequest>();
        if (ttlSeconds > 0)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
            };
            var serialized = JsonSerializer.SerializeToUtf8Bytes(response);
            await _cache.SetAsync(cacheKey, serialized, options, cancellationToken);
            _logger.LogDebug("Cached response for {RequestType} with TTL {Ttl} seconds.", typeof(TRequest).Name, ttlSeconds);
        }

        return response;
    }

    /// <summary>
    /// Generates a cache key using the full name of the request type and a SHA256 hash of the serialized request object.
    /// This ensures that identical queries with the same parameters produce the same key.
    /// </summary>
    private static string GenerateCacheKey(TRequest request)
    {
        // Serialize the request (ignore nulls to keep key stable)
        var serialized = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        var hashBytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(serialized));
        var hash = Convert.ToBase64String(hashBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        return $"{typeof(TRequest).FullName}_{hash}";
    }

    /// <summary>
    /// Retrieves the TTL (in seconds) for the given query type from configuration.
    /// Falls back to the default TTL if no override is specified.
    /// </summary>
    private int GetTtlForQuery<T>()
    {
        var queryName = typeof(T).Name;
        var config = _cacheOptions.CacheableQueries
            .FirstOrDefault(q => string.Equals(q.Name, queryName, StringComparison.OrdinalIgnoreCase));

        return config?.TtlSeconds ?? _cacheOptions.DefaultTtlSeconds;
    }
}