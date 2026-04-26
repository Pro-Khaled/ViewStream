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
    /// Retrieves a paginated list of user interactions for a specific profile.
    /// </summary>
    /// <param name="profileId">The ID of the profile.</param>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of interactions.</returns>
    /// <response code="200">Returns the paginated interactions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet("profile/{profileId:long}")]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    /// Retrieves a paginated list of user interactions for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of interactions.</returns>
    /// <response code="200">Returns the paginated interactions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet("show/{showId:long}")]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    /// Retrieves an interaction summary for a specific profile.
    /// </summary>
    /// <param name="profileId">The ID of the profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Interaction summary (counts by type, top shows).</returns>
    /// <response code="200">Returns the interaction summary.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Profile not found.</response>
    [HttpGet("profile/{profileId:long}/summary")]
    [ProducesResponseType(typeof(UserInteractionSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionSummaryDto>> GetProfileSummary(
        long profileId,
        CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetProfileInteractionSummaryQuery(profileId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }
}