using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Implementation of ISearchEngineService using Meilisearch HTTP endpoints.
    /// Incorporates a graceful fallback to EF Core database search if Meilisearch is not configured or offline.
    /// </summary>
    public class MeilisearchSearchService : ISearchEngineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MeilisearchSearchService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string? _url;
        private readonly string? _apiKey;
        private readonly bool _isEnabled;

        public MeilisearchSearchService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<MeilisearchSearchService> logger,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpClient = httpClient;

            _url = configuration["Meilisearch:Url"];
            _apiKey = configuration["Meilisearch:ApiKey"];
            _isEnabled = !string.IsNullOrEmpty(_url);

            if (_isEnabled)
            {
                _httpClient.BaseAddress = new Uri(_url!);
                if (!string.IsNullOrEmpty(_apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                }
                _logger.LogInformation("Meilisearch Search Service initialized with Url: {Url}", _url);
            }
            else
            {
                _logger.LogWarning("Meilisearch is not configured. Falling back to database LINQ search.");
            }
        }

        public async Task IndexShowAsync(Show show)
        {
            if (!_isEnabled)
            {
                _logger.LogDebug("Meilisearch disabled. Skipping indexing for ShowId {ShowId}.", show.Id);
                return;
            }

            try
            {
                var document = new MeilisearchShowDocument
                {
                    Id = show.Id,
                    Title = show.Title,
                    Description = show.Description,
                    ReleaseYear = show.ReleaseYear,
                    MaturityRating = show.MaturityRating,
                    ImdbRating = show.ImdbRating,
                    RottenTomatoesScore = show.RottenTomatoesScore,
                    Genres = show.Genres.Select(g => g.Name).ToList()
                };

                var response = await _httpClient.PostAsJsonAsync("indexes/shows/documents", new[] { document });
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to index ShowId {ShowId} in Meilisearch: {Error}", show.Id, error);
                }
                else
                {
                    _logger.LogInformation("ShowId {ShowId} indexed in Meilisearch successfully.", show.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while indexing ShowId {ShowId} to Meilisearch.", show.Id);
            }
        }

        public async Task<List<ShowListItemDto>> SearchAsync(string query, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<ShowListItemDto>();
            }

            if (!_isEnabled)
            {
                _logger.LogInformation("Meilisearch disabled. Performing fallback DB search for: {Query}", query);
                return await SearchDatabaseFallbackAsync(query, limit);
            }

            try
            {
                var requestBody = new { q = query, limit };
                var response = await _httpClient.PostAsJsonAsync("indexes/shows/search", requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MeilisearchSearchResult>();
                    if (result?.Hits != null)
                    {
                        var showIds = result.Hits.Select(h => h.Id).ToList();
                        // Retrieve shows in order of match
                        var shows = await _unitOfWork.Shows.GetQueryable()
                            .Where(s => showIds.Contains(s.Id) && s.IsDeleted == false)
                            .Include(s => s.Genres)
                            .ToListAsync();

                        // Sort the results in the exact order returned by Meilisearch hits
                        var sortedShows = showIds
                            .Select(id => shows.FirstOrDefault(s => s.Id == id))
                            .Where(s => s != null)
                            .ToList();

                        return _mapper.Map<List<ShowListItemDto>>(sortedShows);
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Meilisearch search request failed: {Error}", error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching in Meilisearch. Falling back to DB search.");
            }

            return await SearchDatabaseFallbackAsync(query, limit);
        }

        public async Task<List<string>> AutocompleteAsync(string prefix, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return new List<string>();
            }

            if (!_isEnabled)
            {
                return await _unitOfWork.Shows.GetQueryable()
                    .Where(s => s.Title.StartsWith(prefix) && s.IsDeleted == false)
                    .Take(limit)
                    .Select(s => s.Title)
                    .ToListAsync();
            }

            try
            {
                var requestBody = new { q = prefix, limit, attributesToRetrieve = new[] { "title" } };
                var response = await _httpClient.PostAsJsonAsync("indexes/shows/search", requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MeilisearchSearchResult>();
                    if (result?.Hits != null)
                    {
                        return result.Hits.Select(h => h.Title).Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error autocompleting prefix '{Prefix}' in Meilisearch.", prefix);
            }

            // Database fallback
            return await _unitOfWork.Shows.GetQueryable()
                .Where(s => s.Title.Contains(prefix) && s.IsDeleted == false)
                .Take(limit)
                .Select(s => s.Title)
                .ToListAsync();
        }

        public async Task SyncAllShowsAsync()
        {
            if (!_isEnabled)
            {
                _logger.LogWarning("Meilisearch not enabled. Sync skipped.");
                return;
            }

            try
            {
                _logger.LogInformation("Beginning full Meilisearch sync for all shows.");

                // Configure/create index if it does not exist
                var setupResponse = await _httpClient.PostAsJsonAsync("indexes", new { uid = "shows", primaryKey = "id" });
                _logger.LogDebug("Meilisearch index creation status: {StatusCode}", setupResponse.StatusCode);

                var shows = await _unitOfWork.Shows.GetQueryable()
                    .Where(s => s.IsDeleted == false)
                    .Include(s => s.Genres)
                    .ToListAsync();

                var documents = shows.Select(show => new MeilisearchShowDocument
                {
                    Id = show.Id,
                    Title = show.Title,
                    Description = show.Description,
                    ReleaseYear = show.ReleaseYear,
                    MaturityRating = show.MaturityRating,
                    ImdbRating = show.ImdbRating,
                    RottenTomatoesScore = show.RottenTomatoesScore,
                    Genres = show.Genres.Select(g => g.Name).ToList()
                }).ToList();

                var response = await _httpClient.PostAsJsonAsync("indexes/shows/documents", documents);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully synced {Count} shows to Meilisearch.", documents.Count);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to sync shows to Meilisearch: {Error}", error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during full Meilisearch sync.");
            }
        }

        private async Task<List<ShowListItemDto>> SearchDatabaseFallbackAsync(string query, int limit)
        {
            var shows = await _unitOfWork.Shows.GetQueryable()
                .Where(s => s.IsDeleted == false && 
                            (s.Title.Contains(query) || (s.Description != null && s.Description.Contains(query))))
                .Take(limit)
                .Include(s => s.Genres)
                .ToListAsync();

            return _mapper.Map<List<ShowListItemDto>>(shows);
        }
    }

    public class MeilisearchShowDocument
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public short? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
        public decimal? ImdbRating { get; set; }
        public short? RottenTomatoesScore { get; set; }
        public List<string> Genres { get; set; } = new();
    }

    public class MeilisearchSearchResult
    {
        public List<MeilisearchShowDocument> Hits { get; set; } = new();
    }
}
