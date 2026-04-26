using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.CommentReport.CreateCommentReport;
using ViewStream.Application.DTOs;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/comments/{commentId:long}/reports")]
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
    [HttpPost]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            return CreatedAtAction(nameof(AdminCommentReportsController.GetReport), "AdminCommentReports", new { id = report.Id }, report);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    #endregion
}