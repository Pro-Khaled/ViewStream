using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent;
using ViewStream.Application.Commands.PlaybackEvent.DeletePlaybackEvent;
using ViewStream.Application.Commands.PlaybackEvent.PurgeOldPlaybackEvents;
using ViewStream.Application.Queries.PlaybackEvent;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using Microsoft.AspNetCore.RateLimiting;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/playback/events")]
[Authorize]
[Produces("application/json")]
public class PlaybackEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlaybackEventsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Commands

    /// <summary>
    /// Logs a playback event (play, pause, seek, buffer, etc.).
    /// </summary>
    /// <param name="dto">The playback event details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recorded playback event.</returns>
    /// <response code="201">Playback event logged successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("AnalyticsRateLimit")]
    [ProducesResponseType(typeof(PlaybackEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PlaybackEventDto>> LogEvent(
        [FromBody] CreatePlaybackEventDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var evt = await _mediator.Send(new CreatePlaybackEventCommand(profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(LogEvent), null, evt);
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/playbackevents")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminPlaybackEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPlaybackEventsController(IMediator mediator) => _mediator = mediator;

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of playback events for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="episodeId">Optional filter by episode ID.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of playback events.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminPlaybackEventListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    #region Commands

    /// <summary>
    /// Deletes a specific playback event record.
    /// </summary>
    /// <param name="id">The ID of the playback event to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Playback event deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Playback event not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeletePlaybackEvent(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeletePlaybackEventCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Purges playback events older than a specified number of days.
    /// </summary>
    /// <param name="daysToKeep">Number of days of events to retain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of records purged.</returns>
    /// <response code="200">Returns the number of purged events.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("purge")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<int>> PurgeOldPlaybackEvents([FromQuery] int daysToKeep = 30, CancellationToken cancellationToken = default)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PurgeOldPlaybackEventsCommand(daysToKeep, adminUserId), cancellationToken);
        return Ok(result);
    }

    #endregion
}
