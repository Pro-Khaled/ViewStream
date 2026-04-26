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
    /// Retrieves a paginated list of playback events.
    /// </summary>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="episodeId">Optional filter by episode ID.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of playback events.</returns>
    /// <response code="200">Returns the paginated playback events.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PlaybackEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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