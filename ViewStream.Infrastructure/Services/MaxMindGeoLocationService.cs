using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Geo-location service using MaxMind GeoLite2 database.
    /// Falls back to a configurable default country code if the database is not available.
    /// 
    /// To use MaxMind GeoLite2:
    /// 1. Download GeoLite2-Country.mmdb from https://dev.maxmind.com/geoip/geolite2-free-geolocation-data
    /// 2. Place it in the app's data directory or configure the path in appsettings.json
    /// 3. The MaxMind.GeoIP2 NuGet package is already referenced.
    /// </summary>
    public class MaxMindGeoLocationService : IGeoLocationService, IDisposable
    {
        private readonly ILogger<MaxMindGeoLocationService> _logger;
        private readonly DatabaseReader? _reader;
        private readonly string? _defaultCountryCode;

        public MaxMindGeoLocationService(IConfiguration configuration, ILogger<MaxMindGeoLocationService> logger)
        {
            _logger = logger;
            _defaultCountryCode = configuration["MaxMind:DefaultCountryCode"];

            var configuredPath = configuration["MaxMind:DatabasePath"];
            var dbPath = !string.IsNullOrEmpty(configuredPath)
                ? Path.IsPathRooted(configuredPath)
                    ? configuredPath
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuredPath)
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeoLite2-Country.mmdb");

            if (File.Exists(dbPath))
            {
                try
                {
                    _reader = new DatabaseReader(dbPath);
                    _logger.LogInformation("MaxMind GeoLite2 database loaded from {Path}", dbPath);
                }
                catch (InvalidDatabaseException ex)
                {
                    _logger.LogError(ex, "MaxMind database file at {Path} is invalid or corrupted.", dbPath);
                }
            }
            else
            {
                _logger.LogWarning(
                    "GeoLite2-Country.mmdb not found at {Path}. Geo-location will fall back to default country code '{Default}'.",
                    dbPath, _defaultCountryCode ?? "null");
            }
        }

        public Task<string?> GetCountryCodeFromIpAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return Task.FromResult<string?>(_defaultCountryCode);

            // Skip localhost / loopback
            if (ipAddress is "127.0.0.1" or "::1" or "0.0.0.1")
            {
                _logger.LogDebug("Localhost IP detected, returning default country code '{Default}'.", _defaultCountryCode);
                return Task.FromResult<string?>(_defaultCountryCode);
            }

            try
            {
                // Validate the IP address format first
                if (!System.Net.IPAddress.TryParse(ipAddress, out _))
                {
                    _logger.LogWarning("Invalid IP address format: {IP}. Returning default.", ipAddress);
                    return Task.FromResult<string?>(_defaultCountryCode);
                }

                if (_reader != null)
                {
                    var response = _reader.Country(ipAddress);
                    var isoCode = response.Country.IsoCode;

                    if (!string.IsNullOrEmpty(isoCode))
                    {
                        _logger.LogDebug("Resolved IP {IP} to country {Country}", ipAddress, isoCode);
                        return Task.FromResult<string?>(isoCode);
                    }

                    _logger.LogDebug("MaxMind returned no country for IP {IP}. Returning default.", ipAddress);
                    return Task.FromResult<string?>(_defaultCountryCode);
                }

                _logger.LogDebug("MaxMind DB not loaded. Returning default country code for IP: {IP}", ipAddress);
                return Task.FromResult<string?>(_defaultCountryCode);
            }
            catch (AddressNotFoundException)
            {
                _logger.LogDebug("IP {IP} not found in MaxMind database. Returning default.", ipAddress);
                return Task.FromResult<string?>(_defaultCountryCode);
            }
            catch (InvalidDatabaseException ex)
            {
                _logger.LogError(ex, "MaxMind database error while resolving IP: {IP}", ipAddress);
                return Task.FromResult<string?>(_defaultCountryCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve country code for IP: {IP}. Returning default.", ipAddress);
                return Task.FromResult<string?>(_defaultCountryCode);
            }
        }

        public void Dispose()
        {
            _reader?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
