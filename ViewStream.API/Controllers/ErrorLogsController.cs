using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ErrorLog;
using ViewStream.Application.Commands.ErrorLog.DeleteErrorLog;
using ViewStream.Application.Commands.ErrorLog.PurgeOldErrorLogs;
using Microsoft.AspNetCore.RateLimiting;


namespace ViewStream.Api.Controllers;

// Note: ErrorLogs do not have a user-facing side in this system.
// All operations are contained within the Admin controller below.

[ApiController]
[Route("api/v1/admin/errors/logs")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Support")]
[Produces("application/json")]
public class AdminErrorLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminErrorLogsController(IMediator mediator) => _mediator = mediator;

    #region Queries

    /// <summary>
    /// Retrieves a single error log by ID, including the full stack trace and custom data.
    /// </summary>
    /// <param name="id">The error log ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed error information.</returns>
    /// <response code="200">Returns the error log.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Error log not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ErrorLogDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetErrorLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }

    /// <summary>
    /// Retrieves a paginated list of error logs for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="errorCode">Optional filter by error code.</param>
    /// <param name="endpoint">Optional filter by endpoint.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of error logs.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminErrorLogListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminErrorLogListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] string? errorCode = null,
        [FromQuery] string? endpoint = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminErrorLogsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, errorCode, endpoint);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Deletes a specific error log record.
    /// </summary>
    /// <param name="id">The ID of the error log to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Error log deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Error log not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteErrorLog(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteErrorLogCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Purges error logs older than a specified number of days.
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
    public async Task<ActionResult<int>> PurgeOldErrorLogs([FromQuery] int daysToKeep = 30, CancellationToken cancellationToken = default)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PurgeOldErrorLogsCommand(daysToKeep, adminUserId), cancellationToken);
        return Ok(result);
    }

    #endregion
}
