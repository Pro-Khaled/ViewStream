using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.WatchParty.CreateWatchParty;
using ViewStream.Application.Commands.WatchParty.DeleteWatchParty;
using ViewStream.Application.Commands.WatchParty.UpdateWatchParty;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.WatchParty;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/watch-parties")]
[Produces("application/json")]
public class WatchPartiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchPartiesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a watch party by its unique ID.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The watch party if found.</returns>
    /// <response code="200">Returns the watch party.</response>
    /// <response code="404">Party not found.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchPartyDto>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var party = await _mediator.Send(new GetWatchPartyByIdQuery(id), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    /// <summary>
    /// Retrieves a watch party by its unique join code.
    /// </summary>
    /// <param name="code">The party code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active watch party if found.</returns>
    /// <response code="200">Returns the watch party.</response>
    /// <response code="404">Active party not found.</response>
    [HttpGet("code/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchPartyDto>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var party = await _mediator.Send(new GetWatchPartyByCodeQuery(code), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new watch party for a specific episode.
    /// </summary>
    /// <param name="dto">The episode ID to watch together.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created watch party.</returns>
    /// <response code="201">Party created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WatchPartyDto>> Create(
        [FromBody] CreateWatchPartyDto dto,
        CancellationToken cancellationToken)
    {
        var hostProfileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var party = await _mediator.Send(new CreateWatchPartyCommand(hostProfileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = party.Id }, party);
    }

    /// <summary>
    /// Updates a watch party (e.g., deactivate or set end time).
    /// Only the host can update.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="dto">The fields to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated party.</returns>
    /// <response code="200">Party updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the host.</response>
    /// <response code="404">Party not found.</response>
    [HttpPut("{id:long}")]
    [Authorize]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchPartyDto>> Update(
        long id,
        [FromBody] UpdateWatchPartyDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var party = await _mediator.Send(new UpdateWatchPartyCommand(id, profileId, dto, userId), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    /// <summary>
    /// Ends a watch party (sets IsActive = false).
    /// Only the host can end the party.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Party ended successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the host.</response>
    /// <response code="404">Party not found.</response>
    [HttpPost("{id:long}/end")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> End(
        long id,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new EndWatchPartyCommand(id, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}