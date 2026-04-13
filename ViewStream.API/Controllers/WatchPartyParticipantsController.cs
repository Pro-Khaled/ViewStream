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
    private long GetCurrentProfileId() => long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<WatchPartyParticipantDto>>> GetParticipants(long partyId, CancellationToken cancellationToken)
    {
        var participants = await _mediator.Send(new GetParticipantsByPartyQuery(partyId), cancellationToken);
        return Ok(participants);
    }

    [HttpPost("join")]
    public async Task<ActionResult<WatchPartyParticipantDto>> Join(long partyId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var participant = await _mediator.Send(new JoinWatchPartyCommand(partyId, profileId), cancellationToken);
        return Ok(participant);
    }

    [HttpPost("leave")]
    public async Task<IActionResult> Leave(long partyId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new LeaveWatchPartyCommand(partyId, profileId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}