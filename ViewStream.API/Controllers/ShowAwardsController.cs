using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ShowAward.AddShowAward;
using ViewStream.Application.Commands.ShowAward.RemoveShowAward;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ShowAward;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/shows/{showId:long}/awards")]
[Produces("application/json")]
public class ShowAwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ShowAwardsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all awards associated with a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of awards for the show.</returns>
    /// <response code="200">Returns the list of awards.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ShowAwardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShowAwardDto>>> GetAwards(
        long showId,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetShowAwardsQuery(showId), cancellationToken));

    #endregion

    #region Commands

    /// <summary>
    /// Assigns an award to a show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="dto">The award details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created show-award association.</returns>
    /// <response code="201">Award assigned successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="409">Award already assigned to this show.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(ShowAwardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ShowAwardDto>> AddAward(
        long showId,
        [FromBody] CreateShowAwardDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var award = await _mediator.Send(new AddShowAwardCommand(showId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetAwards), new { showId }, award);
    }

    /// <summary>
    /// Removes an award assignment from a show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="awardId">The ID of the award to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Award removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Association not found.</response>
    [HttpDelete("{awardId:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAward(
        long showId,
        int awardId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RemoveShowAwardCommand(showId, awardId, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}