using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.CommentReport.UpdateCommentReport;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.CommentReport;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/admin/reports/comments")]
[Authorize(Roles = "SuperAdmin,Moderator")]
[Produces("application/json")]
public class AdminCommentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCommentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of comment reports.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="status">Optional filter by report status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of comment reports.</returns>
    /// <response code="200">Returns the paginated reports.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(PagedResult<CommentReportListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<CommentReportListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetReportsPagedQuery(page, pageSize, status), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single comment report by ID.
    /// </summary>
    /// <param name="id">The ID of the report.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full report details.</returns>
    /// <response code="200">Returns the report.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Report not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CommentReportDto>> GetReport(
        long id,
        CancellationToken cancellationToken)
    {
        var report = await _mediator.Send(new GetReportByIdQuery(id), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

    
        /// <summary>
        /// Retrieves a paginated list of comment reports for the admin dashboard.
        /// </summary>
        /// <param name="pageNumber">Page number (1-indexed).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
        /// <param name="status">Optional filter by status.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of commentreports.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
        [HttpGet("api/admin/reports/comments")]
    [EnableRateLimiting("AdminRateLimit")]
        [Authorize(Roles = "SuperAdmin,Moderator")]
        [ProducesResponseType(typeof(PagedResult<AdminCommentReportListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<PagedResult<AdminCommentReportListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] string? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminCommentReportsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, status);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    #endregion

    #region Commands

    /// <summary>
    /// Updates the status of a comment report (e.g., reviewed, dismissed, action taken).
    /// </summary>
    /// <param name="id">The ID of the report.</param>
    /// <param name="dto">The new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated report.</returns>
    /// <response code="200">Status updated successfully.</response>
    /// <response code="400">Invalid status value.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Report not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}/status")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CommentReportDto>> UpdateStatus(
        long id,
        [FromBody] UpdateReportStatusDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var report = await _mediator.Send(new UpdateReportStatusCommand(id, dto, userId), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

    #endregion
}
