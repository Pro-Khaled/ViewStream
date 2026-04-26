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
    [HttpPost]
    [ProducesResponseType(typeof(ContentReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            return CreatedAtAction(nameof(AdminContentReportsController.GetReport), "AdminContentReports", new { id = report.Id }, report);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    #endregion
}