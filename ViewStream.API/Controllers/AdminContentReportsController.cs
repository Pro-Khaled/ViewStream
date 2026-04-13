using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    /// Gets a paginated list of content reports (admin only).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContentReportListItemDto>), StatusCodes.Status200OK)]
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
    /// Gets a single report by ID (admin only).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentReportDto>> GetReport(long id, CancellationToken cancellationToken)
    {
        var report = await _mediator.Send(new GetContentReportByIdQuery(id), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Updates the status of a content report (admin only).
    /// </summary>
    [HttpPut("{id:long}/status")]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentReportDto>> UpdateStatus(long id, [FromBody] UpdateContentReportStatusDto dto, CancellationToken cancellationToken)
    {
        var report = await _mediator.Send(new UpdateContentReportStatusCommand(id, dto), cancellationToken);
        if (report == null) return NotFound();
        return Ok(report);
    }
}