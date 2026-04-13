using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ContentReport.CreateContentReport;
using ViewStream.Application.DTOs;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/reports/content")]
[Authorize]
[Produces("application/json")]
public class ContentReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    /// <summary>
    /// Reports a show or episode for moderation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReportContent([FromBody] CreateContentReportDto dto, CancellationToken cancellationToken)
    {
        if (!dto.ShowId.HasValue && !dto.EpisodeId.HasValue)
            return BadRequest("Either ShowId or EpisodeId must be provided.");

        var profileId = GetCurrentProfileId();
        var report = await _mediator.Send(new CreateContentReportCommand(profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(AdminContentReportsController.GetReport), "AdminContentReports", new { id = report.Id }, report);
    }
}