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

    /// <summary>
    /// Reports a comment for moderation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommentReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReportComment(long commentId, [FromBody] CreateCommentReportDto dto, CancellationToken cancellationToken)
    {
        if (commentId != dto.CommentId)
            return BadRequest("Comment ID mismatch.");

        var profileId = GetCurrentProfileId();
        var report = await _mediator.Send(new CreateCommentReportCommand(profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(AdminCommentReportsController.GetReport), "AdminCommentReports", new { id = report.Id }, report);
    }
}

