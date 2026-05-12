using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PlaybackEvent;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/admin/playback/events")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminPlaybackEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPlaybackEventsController(IMediator mediator) => _mediator = mediator;

    #region Queries
    /// <summary>
    /// Retrieves a paginated list of playback events.
    /// </summary>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="episodeId">Optional filter by episode ID.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of playback events.</returns>
    /// <response code="200">Returns the paginated playback events.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(PagedResult<PlaybackEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<PlaybackEventDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] long? episodeId = null,
        [FromQuery] long? profileId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPlaybackEventsPagedQuery(page, pageSize, episodeId, profileId), cancellationToken);
        return Ok(result);
    }

    
        /// <summary>
        /// Retrieves a paginated list of playback events for the admin dashboard.
        /// </summary>
        /// <param name="pageNumber">Page number (1-indexed).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
        /// <param name="episodeId">Optional filter by episodeid.</param>
        /// <param name="profileId">Optional filter by profileid.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of playbackevents.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
        [HttpGet("api/admin/playback/events")]
    [EnableRateLimiting("AdminRateLimit")]
        [Authorize(Roles = "SuperAdmin,Analytics")]
        [ProducesResponseType(typeof(PagedResult<AdminPlaybackEventListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<PagedResult<AdminPlaybackEventListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? episodeId = null,
        [FromQuery] long? profileId = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminPlaybackEventsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, episodeId, profileId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    #endregion
}
