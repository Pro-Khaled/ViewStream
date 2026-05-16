using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ContentReport.CreateContentReport;
using ViewStream.Application.Commands.ContentReport.UpdateContentReport;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ContentReport;
using Microsoft.AspNetCore.RateLimiting;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/reports/content")]
[Authorize]
[Produces("application/json")]
public class ContentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Commands

    /// <summary>
    /// Submits a report against a show or episode for moderation review.
    /// </summary>
    /// <param name="dto">The report details including target (ShowId or EpisodeId) and reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created content report.</returns>
    /// <response code="201">Report submitted successfully.</response>
    /// <response code="400">Invalid input (neither ShowId nor EpisodeId provided).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="409">User has already reported this content.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ReportRateLimit")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ReportContent(
        [FromBody] CreateContentReportDto dto,
        CancellationToken cancellationToken)
    {
        if (!dto.ShowId.HasValue && !dto.EpisodeId.HasValue)
            return BadRequest("Either ShowId or EpisodeId must be provided.");

        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();

        try
        {
            var report = await _mediator.Send(new CreateContentReportCommand(profileId, dto, userId), cancellationToken);
            return CreatedAtAction("GetReport", "AdminContentReports", new { id = report.Id }, report);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/contentreports")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Moderator")]
[Produces("application/json")]
public class AdminContentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminContentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Retrieves a paginated list of content reports for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="status">Optional filter by status.</param>
    /// <param name="targetType">Optional filter by targetType.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of content reports.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminContentReportListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminContentReportListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] string? status = null,
        [FromQuery] string? targetType = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminContentReportsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, status, targetType);
        var result = await _mediator.Send(query, cancellationToken);
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ContentReportDto>> GetReport(
        long id,
        CancellationToken cancellationToken)
    {
        var report = await _mediator.Send(new GetContentReportByIdQuery(id), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}/status")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
}
