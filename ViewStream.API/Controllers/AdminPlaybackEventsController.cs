using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PlaybackEvent;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/playback/events")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminPlaybackEventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPlaybackEventsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets paginated playback events (admin only).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PlaybackEventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PlaybackEventDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] long? episodeId = null,
        [FromQuery] long? profileId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPlaybackEventsPagedQuery(page, pageSize, episodeId, profileId), cancellationToken);
        return Ok(result);
    }
}