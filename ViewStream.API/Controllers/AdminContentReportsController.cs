using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ContentReport.UpdateContentReport;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ContentReport;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/reports/content")]
[Authorize(Roles = "SuperAdmin,Moderator")]
[Produces("application/json")]
public class AdminContentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminContentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of content reports.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="status">Optional filter by report status.</param>
    /// <param name="targetType">Optional filter by target type ("Show" or "Episode").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of content reports.</returns>
    /// <response code="200">Returns the paginated reports.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContentReportListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ContentReportListItemDto>>> GetReports(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? targetType = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetContentReportsPagedQuery(page, pageSize, status, targetType), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single content report by ID.
    /// </summary>
    /// <param name="id">The ID of the report.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full report details.</returns>
    /// <response code="200">Returns the report.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Report not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentReportDto>> GetReport(
        long id,
        CancellationToken cancellationToken)
    {
        var report = await _mediator.Send(new GetContentReportByIdQuery(id), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Updates the status of a content report (e.g., reviewed, dismissed, action taken).
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
    [HttpPut("{id:long}/status")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentReportDto>> UpdateStatus(
        long id,
        [FromBody] UpdateContentReportStatusDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var report = await _mediator.Send(new UpdateContentReportStatusCommand(id, dto, userId), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

    #endregion
}