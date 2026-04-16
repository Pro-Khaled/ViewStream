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

    /// <summary>
    /// Logs a playback event (play, pause, seek, buffer, etc.).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PlaybackEventDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PlaybackEventDto>> LogEvent([FromBody] CreatePlaybackEventDto dto, CancellationToken cancellationToken)
    {
        var evt = await _mediator.Send(new CreatePlaybackEventCommand(GetCurrentProfileId(), dto), cancellationToken);
        return CreatedAtAction(nameof(LogEvent), null, evt);
    }
}