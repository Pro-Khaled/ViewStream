using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.AuditLog;
using ViewStream.Application.Commands.AuditLog.DeleteAuditLog;
using ViewStream.Application.Commands.AuditLog.PurgeOldAuditLogs;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

// Note: AuditLogs do not have a user-facing side in this system.
// All operations are contained within the Admin controller below.

[ApiController]
[Route("api/v1/admin/audit/logs")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Auditor")]
[Produces("application/json")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminAuditLogsController(IMediator mediator) => _mediator = mediator;

    #region Queries

    /// <summary>
    /// Gets a specific audit log by ID (includes full old/new values).
    /// </summary>
    /// <param name="id">The ID of the audit log.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested audit log.</returns>
    /// <response code="200">Returns the audit log.</response>
    /// <response code="404">Audit log not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuditLogDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetAuditLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }

    /// <summary>
    /// Retrieves a paginated list of audit logs for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="tableName">Optional filter by table name.</param>
    /// <param name="recordId">Optional filter by record ID.</param>
    /// <param name="action">Optional filter by action.</param>
    /// <param name="changedByUserId">Optional filter by changed by user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of audit logs.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminAuditLogListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminAuditLogListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] string? tableName = null,
        [FromQuery] long? recordId = null,
        [FromQuery] string? action = null,
        [FromQuery] long? changedByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminAuditLogsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, tableName, recordId, action, changedByUserId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Deletes a specific audit log record.
    /// </summary>
    /// <param name="id">The ID of the audit log to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Audit log deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Audit log not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteAuditLog(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteAuditLogCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Purges audit logs older than a specified number of days.
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
    public async Task<ActionResult<int>> PurgeOldAuditLogs([FromQuery] int daysToKeep = 30, CancellationToken cancellationToken = default)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PurgeOldAuditLogsCommand(daysToKeep, adminUserId), cancellationToken);
        return Ok(result);
    }

    #endregion
}
