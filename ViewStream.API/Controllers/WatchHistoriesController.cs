using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory;
using ViewStream.Application.Commands.WatchHistory.DeleteWatchHistory;
using ViewStream.Application.Commands.WatchHistory.PurgeOldWatchHistories;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.WatchHistory;
using Microsoft.AspNetCore.RateLimiting;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/profiles/me/history")]
[Authorize]
[Produces("application/json")]
public class WatchHistoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchHistoriesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated watch history for the current profile, sorted by most recently watched.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of watch history items.</returns>
    /// <response code="200">Returns the paged watch history.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<WatchHistoryListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<WatchHistoryListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new GetWatchHistoryPagedQuery(profileId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the "Continue Watching" list for the current profile.
    /// </summary>
    /// <param name="limit">Maximum number of items to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of partially watched episodes.</returns>
    /// <response code="200">Returns the continue watching list.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("continue")]
    [ProducesResponseType(typeof(List<WatchHistoryListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WatchHistoryListItemDto>>> GetContinueWatching(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new GetContinueWatchingQuery(profileId, limit), cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Records or updates watch progress for an episode.
    /// </summary>
    /// <param name="dto">Episode ID, progress in seconds, and completed flag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upserted watch history record.</returns>
    /// <response code="200">Progress recorded successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(WatchHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<WatchHistoryDto>> Upsert(
        [FromBody] CreateUpdateWatchHistoryDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpsertWatchHistoryCommand(profileId, dto, userId), cancellationToken);
        return Ok(result);
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/watchhistory")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminWatchHistoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminWatchHistoriesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of watch histories for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="episodeId">Optional filter by episode ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of watch histories.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminWatchHistoryListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminWatchHistoryListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? profileId = null,
        [FromQuery] long? episodeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminWatchHistoriesPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, profileId, episodeId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a specific watch history record.
    /// </summary>
    /// <param name="id">The ID of the watch history to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Watch history deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Watch history not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteWatchHistory(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ViewStream.Application.Commands.WatchHistory.DeleteWatchHistory.DeleteWatchHistoryCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Purges watch histories older than a specified number of days.
    /// </summary>
    /// <param name="daysToKeep">Number of days of history to retain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of records purged.</returns>
    /// <response code="200">Returns the number of purged history items.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("purge")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<int>> PurgeOldWatchHistories([FromQuery] int daysToKeep = 90, CancellationToken cancellationToken = default)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ViewStream.Application.Commands.WatchHistory.PurgeOldWatchHistories.PurgeOldWatchHistoriesCommand(daysToKeep, adminUserId), cancellationToken);
        return Ok(result);
    }
}
