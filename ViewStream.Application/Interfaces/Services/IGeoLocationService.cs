namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Resolves the user's country from their IP address using a geo-location database.
    /// </summary>
    public interface IGeoLocationService
    {
        /// <summary>
        /// Returns the ISO 3166-1 alpha-2 country code for the given IP address, or null if unknown.
        /// </summary>
        Task<string?> GetCountryCodeFromIpAsync(string ipAddress);
    }
}
