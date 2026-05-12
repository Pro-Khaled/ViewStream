using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.AuditLog;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/admin/audit/logs")]
[Authorize(Roles = "SuperAdmin,Auditor")]
[Produces("application/json")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminAuditLogsController(IMediator mediator) => _mediator = mediator;

    #region Queries
    /// <summary>
    /// Gets paginated audit logs with extensive filters.
    /// </summary>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(PagedResult<AuditLogListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AuditLogListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tableName = null,
        [FromQuery] long? recordId = null,
        [FromQuery] string? action = null,
        [FromQuery] long? changedByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuditLogsPagedQuery(page, pageSize, tableName, recordId, action, changedByUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific audit log by ID (includes full old/new values).
    /// </summary>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("AdminRateLimit")]
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
        /// <param name="tableName">Optional filter by tablename.</param>
        /// <param name="recordId">Optional filter by recordid.</param>
        /// <param name="action">Optional filter by action.</param>
        /// <param name="changedByUserId">Optional filter by changedbyuserid.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of auditlogs.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
        [HttpGet("api/admin/audit/logs")]
        [EnableRateLimiting("AdminRateLimit")]
        [Authorize(Roles = "SuperAdmin,Auditor")]
        [ProducesResponseType(typeof(PagedResult<AdminAuditLogListItemDto>), StatusCodes.Status200OK)]
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
}
