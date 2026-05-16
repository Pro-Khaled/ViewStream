using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.CommentReport.CreateCommentReport;
using ViewStream.Application.Commands.CommentReport.UpdateCommentReport;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.CommentReport;
using Microsoft.AspNetCore.RateLimiting;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/comments/{commentId:long}/reports")]
[Authorize]
[Produces("application/json")]
public class CommentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Commands

    /// <summary>
    /// Submits a report against a comment for moderation review.
    /// </summary>
    /// <param name="commentId">The ID of the comment being reported.</param>
    /// <param name="dto">The report details including reason and optional description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created report.</returns>
    /// <response code="201">Report submitted successfully.</response>
    /// <response code="400">Comment ID mismatch or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="409">User has already reported this comment.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ReportRateLimit")]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ReportComment(
        long commentId,
        [FromBody] CreateCommentReportDto dto,
        CancellationToken cancellationToken)
    {
        if (commentId != dto.CommentId)
            return BadRequest("Comment ID mismatch.");

        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();

        try
        {
            var report = await _mediator.Send(new CreateCommentReportCommand(profileId, dto, userId), cancellationToken);
            return CreatedAtAction("GetReport", "AdminCommentReports", new { id = report.Id }, report);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/commentreports")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Moderator")]
[Produces("application/json")]
public class AdminCommentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCommentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

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
    /// <returns>A paginated list of comment reports.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminCommentReportListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
}
