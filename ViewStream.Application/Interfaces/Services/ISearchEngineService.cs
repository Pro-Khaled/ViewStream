using System.Collections.Generic;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;

namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Abstraction for a fast full-text search engine (e.g. Meilisearch / Elasticsearch).
    /// </summary>
    public interface ISearchEngineService
    {
        /// <summary>Indexes a single show into the search engine.</summary>
        Task IndexShowAsync(Show show);

        /// <summary>Searches for shows matching the query.</summary>
        Task<List<ShowListItemDto>> SearchAsync(string query, int limit = 20);

        /// <summary>Generates autocomplete query suggestions matching the prefix.</summary>
        Task<List<string>> AutocompleteAsync(string prefix, int limit = 10);

        /// <summary>Indexes/syncs all active shows from database to the search engine.</summary>
        Task SyncAllShowsAsync();
    }
}
