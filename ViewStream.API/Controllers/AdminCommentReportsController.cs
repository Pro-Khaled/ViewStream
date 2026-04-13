using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.CommentReport.UpdateCommentReport;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.CommentReport;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/reports/comments")]
[Authorize(Roles = "SuperAdmin,Moderator")]
[Produces("application/json")]
public class AdminCommentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCommentReportsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Gets a paginated list of comment reports (admin only).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CommentReportListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CommentReportListItemDto>>> GetReports(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetReportsPagedQuery(page, pageSize, status), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single report by ID (admin only).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentReportDto>> GetReport(long id, CancellationToken cancellationToken)
    {
        var report = await _mediator.Send(new GetReportByIdQuery(id), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Updates the status of a report (admin only).
    /// </summary>
    [HttpPut("{id:long}/status")]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentReportDto>> UpdateStatus(long id, [FromBody] UpdateReportStatusDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var report = await _mediator.Send(new UpdateReportStatusCommand(id, dto, userId), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }
}