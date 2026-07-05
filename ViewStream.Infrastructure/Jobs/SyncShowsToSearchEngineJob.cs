using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Hangfire recurring job that syncs all catalogues from DB to the search engine.
    /// Runs daily at 4 AM.
    /// </summary>
    public class SyncShowsToSearchEngineJob
    {
        private readonly ISearchEngineService _searchEngineService;
        private readonly ILogger<SyncShowsToSearchEngineJob> _logger;

        public SyncShowsToSearchEngineJob(ISearchEngineService searchEngineService, ILogger<SyncShowsToSearchEngineJob> logger)
        {
            _searchEngineService = searchEngineService;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Starting SyncShowsToSearchEngineJob execution...");
            try
            {
                await _searchEngineService.SyncAllShowsAsync();
                _logger.LogInformation("SyncShowsToSearchEngineJob completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncShowsToSearchEngineJob failed.");
                throw;
            }
        }
    }
}
