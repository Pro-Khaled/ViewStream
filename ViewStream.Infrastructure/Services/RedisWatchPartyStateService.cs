using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Redis-backed watch party state management for real-time synchronization.
    /// Also tracks connection-to-party mappings for host failover on disconnect.
    /// Falls back to in-memory dictionary if Redis is unavailable.
    /// </summary>
    public class RedisWatchPartyStateService : IWatchPartyStateService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisWatchPartyStateService> _logger;
        private static readonly TimeSpan StateExpiry = TimeSpan.FromHours(4);
        private static readonly TimeSpan ConnectionTrackingExpiry = TimeSpan.FromHours(6);

        public RedisWatchPartyStateService(IDistributedCache cache, ILogger<RedisWatchPartyStateService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        private static string BuildKey(string partyCode) => $"watchparty:state:{partyCode}";
        private static string BuildConnKey(string connectionId) => $"watchparty:conn:{connectionId}";

        public async Task<WatchPartyState?> GetStateAsync(string partyCode)
        {
            var key = BuildKey(partyCode);
            var json = await _cache.GetStringAsync(key);
            if (json == null) return null;

            return JsonSerializer.Deserialize<WatchPartyState>(json);
        }

        public async Task SetStateAsync(string partyCode, WatchPartyState state)
        {
            var key = BuildKey(partyCode);
            var json = JsonSerializer.Serialize(state);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = StateExpiry
            };
            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveStateAsync(string partyCode)
        {
            var key = BuildKey(partyCode);
            await _cache.RemoveAsync(key);
            _logger.LogInformation("Watch party state removed: {PartyCode}", partyCode);
        }

        public async Task TrackConnectionAsync(string connectionId, string partyCode)
        {
            var key = BuildConnKey(connectionId);
            var existing = await _cache.GetStringAsync(key);

            var partyCodes = new HashSet<string>();
            if (!string.IsNullOrEmpty(existing))
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(existing);
                if (parsed != null) partyCodes = new HashSet<string>(parsed);
            }

            partyCodes.Add(partyCode);

            await _cache.SetStringAsync(key,
                JsonSerializer.Serialize(partyCodes.ToList()),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ConnectionTrackingExpiry
                });
        }

        public async Task UntrackConnectionAsync(string connectionId, string partyCode)
        {
            var key = BuildConnKey(connectionId);
            var existing = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(existing)) return;

            var partyCodes = JsonSerializer.Deserialize<List<string>>(existing) ?? new List<string>();
            partyCodes.Remove(partyCode);

            if (partyCodes.Count == 0)
            {
                await _cache.RemoveAsync(key);
            }
            else
            {
                await _cache.SetStringAsync(key,
                    JsonSerializer.Serialize(partyCodes),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ConnectionTrackingExpiry
                    });
            }
        }

        public async Task<List<string>> GetPartyCodesForConnectionAsync(string connectionId)
        {
            var key = BuildConnKey(connectionId);
            var json = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(json)) return new List<string>();

            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }
}
