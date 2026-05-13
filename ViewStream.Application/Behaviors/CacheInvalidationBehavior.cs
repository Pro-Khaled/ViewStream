using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Behaviors
{
    /// <summary>
    /// Automatically invalidates cache patterns after command handlers that modify data.
    /// </summary>
    public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheInvalidator _invalidator;
        private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

        public CacheInvalidationBehavior(ICacheInvalidator invalidator, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
        {
            _invalidator = invalidator;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Execute the command first
            var response = await next();

            // If the command succeeded (optional: check response for success flag), invalidate patterns
            if (CacheInvalidationMappings.Mappings.TryGetValue(typeof(TRequest), out var patterns))
            {
                var entityId = ExtractEntityId(request);
                foreach (var pattern in patterns)
                {
                    var resolvedPattern = pattern.Replace("{Id}", entityId?.ToString() ?? "*");
                    _logger.LogDebug("Invalidating cache pattern: {Pattern} for command {CommandType}", resolvedPattern, typeof(TRequest).Name);
                    await _invalidator.InvalidateByPatternAsync(resolvedPattern, cancellationToken);
                }
            }

            return response;
        }

        /// <summary>
        /// Extracts the primary entity ID from the command using common naming conventions.
        /// Supports commands that have an Id property or a Dto with an Id.
        /// </summary>
        private static object? ExtractEntityId(TRequest command)
        {
            var type = typeof(TRequest);

            // 1. Try direct property "Id" (most common)
            var idProp = type.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (idProp != null)
                return idProp.GetValue(command);

            // 2. Try direct property "ReportId" (used by comment/content report commands)
            var reportIdProp = type.GetProperty("ReportId", BindingFlags.Public | BindingFlags.Instance);
            if (reportIdProp != null)
                return reportIdProp.GetValue(command);

            // 3. Try direct property "PartyId" (used by watch party participant commands)
            var partyIdProp = type.GetProperty("PartyId", BindingFlags.Public | BindingFlags.Instance);
            if (partyIdProp != null)
                return partyIdProp.GetValue(command);

            // 4. Try direct property "ListId" (used by shared list commands)
            var listIdProp = type.GetProperty("ListId", BindingFlags.Public | BindingFlags.Instance);
            if (listIdProp != null)
                return listIdProp.GetValue(command);

            // 5. Try direct property "SeasonId"
            var seasonIdProp = type.GetProperty("SeasonId", BindingFlags.Public | BindingFlags.Instance);
            if (seasonIdProp != null)
                return seasonIdProp.GetValue(command);

            // 6. Try direct property "EpisodeId"
            var episodeIdProp = type.GetProperty("EpisodeId", BindingFlags.Public | BindingFlags.Instance);
            if (episodeIdProp != null)
                return episodeIdProp.GetValue(command);

            // 7. Try direct property "ShowId"
            var showIdProp = type.GetProperty("ShowId", BindingFlags.Public | BindingFlags.Instance);
            if (showIdProp != null)
                return showIdProp.GetValue(command);

            // 8. Try direct property "PersonId"
            var personIdProp = type.GetProperty("PersonId", BindingFlags.Public | BindingFlags.Instance);
            if (personIdProp != null)
                return personIdProp.GetValue(command);

            // 9. Try direct property "AwardId"
            var awardIdProp = type.GetProperty("AwardId", BindingFlags.Public | BindingFlags.Instance);
            if (awardIdProp != null)
                return awardIdProp.GetValue(command);

            // 10. Try direct property "PromoCodeId"
            var promoCodeIdProp = type.GetProperty("PromoCodeId", BindingFlags.Public | BindingFlags.Instance);
            if (promoCodeIdProp != null)
                return promoCodeIdProp.GetValue(command);

            // 11. Try direct property "UserId"
            var userIdProp = type.GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance);
            if (userIdProp != null)
                return userIdProp.GetValue(command);

            // 12. Try direct property "RoleId"
            var roleIdProp = type.GetProperty("RoleId", BindingFlags.Public | BindingFlags.Instance);
            if (roleIdProp != null)
                return roleIdProp.GetValue(command);

            // 13. Try direct property "ProfileId"
            var profileIdProp = type.GetProperty("ProfileId", BindingFlags.Public | BindingFlags.Instance);
            if (profileIdProp != null)
                return profileIdProp.GetValue(command);

            // 14. Try "Dto" pattern (if the command has a DTO with an Id)
            var dtoProp = type.GetProperty("Dto", BindingFlags.Public | BindingFlags.Instance);
            if (dtoProp != null)
            {
                var dto = dtoProp.GetValue(command);
                if (dto != null)
                {
                    var dtoType = dto.GetType();
                    // Look for Id, ShowId, SeasonId, EpisodeId, ReportId, etc.
                    var dtoIdProp = dtoType.GetProperty("Id");
                    if (dtoIdProp != null)
                        return dtoIdProp.GetValue(dto);
                    var dtoShowIdProp = dtoType.GetProperty("ShowId");
                    if (dtoShowIdProp != null)
                        return dtoShowIdProp.GetValue(dto);
                    var dtoSeasonIdProp = dtoType.GetProperty("SeasonId");
                    if (dtoSeasonIdProp != null)
                        return dtoSeasonIdProp.GetValue(dto);
                    var dtoEpisodeIdProp = dtoType.GetProperty("EpisodeId");
                    if (dtoEpisodeIdProp != null)
                        return dtoEpisodeIdProp.GetValue(dto);
                }
            }

            // If nothing found, return null – placeholder becomes '*' (wildcard)
            return null;
        }
    }
}

