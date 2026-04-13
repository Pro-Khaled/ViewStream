using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserInteraction;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/interactions")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminUserInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUserInteractionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets interactions by profile (admin only).
    /// </summary>
    [HttpGet("profile/{profileId:long}")]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserInteractionListItemDto>>> GetByProfile(
        long profileId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInteractionsByProfileQuery(profileId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets interactions by show (admin only).
    /// </summary>
    [HttpGet("show/{showId:long}")]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserInteractionListItemDto>>> GetByShow(
        long showId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInteractionsByShowQuery(showId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets interaction summary for any profile (admin only).
    /// </summary>
    [HttpGet("profile/{profileId:long}/summary")]
    [ProducesResponseType(typeof(UserInteractionSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionSummaryDto>> GetProfileSummary(long profileId, CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetProfileInteractionSummaryQuery(profileId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }
}