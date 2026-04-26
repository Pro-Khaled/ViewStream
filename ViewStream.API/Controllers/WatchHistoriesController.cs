using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.WatchHistory;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/history")]
[Authorize]
[Produces("application/json")]
public class WatchHistoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchHistoriesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the "Continue Watching" list for the current profile.
    /// </summary>
    /// <param name="limit">Maximum number of items to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of partially watched episodes.</returns>
    /// <response code="200">Returns the continue watching list.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("continue")]
    [ProducesResponseType(typeof(List<WatchHistoryListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WatchHistoryListItemDto>>> GetContinueWatching(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new GetContinueWatchingQuery(profileId, limit), cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Records or updates watch progress for an episode.
    /// </summary>
    /// <param name="dto">Episode ID, progress in seconds, and completed flag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upserted watch history record.</returns>
    /// <response code="200">Progress recorded successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(WatchHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WatchHistoryDto>> Upsert(
        [FromBody] CreateUpdateWatchHistoryDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpsertWatchHistoryCommand(profileId, dto, userId), cancellationToken);
        return Ok(result);
    }

    #endregion
}