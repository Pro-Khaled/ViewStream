using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ViewStream.Application.Interfaces.Services
{
    public class RedisCacheInvalidator : ICacheInvalidator
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connection;

        public RedisCacheInvalidator(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _database = connection.GetDatabase();
        }

        public async Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            var endpoints = _connection.GetEndPoints();
            var server = _connection.GetServer(endpoints.First());
            var keys = server.Keys(pattern: pattern).ToArray();
            if (keys.Length > 0)
                await _database.KeyDeleteAsync(keys);
        }
    }
}
