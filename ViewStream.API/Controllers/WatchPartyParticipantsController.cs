using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.WatchPartyParticipant.JoinWatchParty;
using ViewStream.Application.Commands.WatchPartyParticipant.LeaveWatchParty;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.WatchPartyParticipant;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/watch-parties/{partyId:long}/participants")]
[Authorize]
[Produces("application/json")]
public class WatchPartyParticipantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchPartyParticipantsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all participants of a watch party.
    /// </summary>
    /// <param name="partyId">The ID of the watch party.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of participants with join/leave times.</returns>
    /// <response code="200">Returns the list of participants.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<WatchPartyParticipantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WatchPartyParticipantDto>>> GetParticipants(
        long partyId,
        CancellationToken cancellationToken)
    {
        var participants = await _mediator.Send(new GetParticipantsByPartyQuery(partyId), cancellationToken);
        return Ok(participants);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Joins a watch party.
    /// </summary>
    /// <param name="partyId">The ID of the watch party to join.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The participant record.</returns>
    /// <response code="200">Joined successfully (or already joined).</response>
    /// <response code="400">Party not found or inactive.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("join")]
    [ProducesResponseType(typeof(WatchPartyParticipantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WatchPartyParticipantDto>> Join(
        long partyId,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var participant = await _mediator.Send(new JoinWatchPartyCommand(partyId, profileId, userId), cancellationToken);
        return Ok(participant);
    }

    /// <summary>
    /// Leaves a watch party.
    /// </summary>
    /// <param name="partyId">The ID of the watch party.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Left successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Participant record not found.</response>
    [HttpPost("leave")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Leave(
        long partyId,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new LeaveWatchPartyCommand(partyId, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}