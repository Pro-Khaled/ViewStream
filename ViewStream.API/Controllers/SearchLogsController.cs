using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.SearchLog;
using ViewStream.Application.Commands.SearchLog.DeleteSearchLog;
using ViewStream.Application.Commands.SearchLog.PurgeOldSearchLogs;
using Microsoft.AspNetCore.RateLimiting;


namespace ViewStream.Api.Controllers;

// Note: SearchLogs do not have a user-facing side in this system.
// All operations are contained within the Admin controller below.

[ApiController]
[Route("api/v1/admin/search/logs")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminSearchLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSearchLogsController(IMediator mediator) => _mediator = mediator;

    #region Queries

    /// <summary>
    /// Retrieves a single search log entry by its ID.
    /// </summary>
    /// <param name="id">The ID of the search log.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detailed search log entry.</returns>
    /// <response code="200">Returns the search log.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Search log not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SearchLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SearchLogDto>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetSearchLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }

    /// <summary>
    /// Retrieves a paginated list of search logs for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="query">Optional filter by search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of search logs.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminSearchLogListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminSearchLogListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? profileId = null,
        [FromQuery] string? query = null,
        CancellationToken cancellationToken = default)
    {
        var logsQuery = new GetAdminSearchLogsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, profileId, query);
        var result = await _mediator.Send(logsQuery, cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Deletes a specific search log record.
    /// </summary>
    /// <param name="id">The ID of the search log to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Search log deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Search log not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteSearchLog(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteSearchLogCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Purges search logs older than a specified number of days.
    /// </summary>
    /// <param name="daysToKeep">Number of days of logs to retain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of records purged.</returns>
    /// <response code="200">Returns the number of purged logs.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("purge")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<int>> PurgeOldSearchLogs([FromQuery] int daysToKeep = 30, CancellationToken cancellationToken = default)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PurgeOldSearchLogsCommand(daysToKeep, adminUserId), cancellationToken);
        return Ok(result);
    }

    #endregion
}
