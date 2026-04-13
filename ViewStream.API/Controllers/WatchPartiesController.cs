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
[Authorize]
[Produces("application/json")]
public class WatchPartiesController : ControllerBase
{
    private readonly IMediator _mediator;
    public WatchPartiesController(IMediator mediator) => _mediator = mediator;
    private long GetCurrentProfileId() => long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    [HttpPost]
    public async Task<ActionResult<WatchPartyDto>> Create([FromBody] CreateWatchPartyDto dto, CancellationToken cancellationToken)
    {
        var hostProfileId = GetCurrentProfileId();
        var party = await _mediator.Send(new CreateWatchPartyCommand(hostProfileId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = party.Id }, party);
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<WatchPartyDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var party = await _mediator.Send(new GetWatchPartyByIdQuery(id), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    [HttpGet("code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<WatchPartyDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var party = await _mediator.Send(new GetWatchPartyByCodeQuery(code), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<WatchPartyDto>> Update(long id, [FromBody] UpdateWatchPartyDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var party = await _mediator.Send(new UpdateWatchPartyCommand(id, profileId, dto), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    [HttpPost("{id:long}/end")]
    public async Task<IActionResult> End(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new EndWatchPartyCommand(id, profileId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}