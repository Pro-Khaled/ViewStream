using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent;
using ViewStream.Application.DTOs;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/playback/events")]
[Authorize]
[Produces("application/json")]
public class PlaybackEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlaybackEventsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Commands

    /// <summary>
    /// Logs a playback event (play, pause, seek, buffer, etc.).
    /// </summary>
    /// <param name="dto">The playback event details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recorded playback event.</returns>
    /// <response code="201">Playback event logged successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PlaybackEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PlaybackEventDto>> LogEvent(
        [FromBody] CreatePlaybackEventDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var evt = await _mediator.Send(new CreatePlaybackEventCommand(profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(LogEvent), null, evt);
    }

    #endregion
}