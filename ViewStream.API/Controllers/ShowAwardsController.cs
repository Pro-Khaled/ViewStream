using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ShowAwardDto>>> GetAwards(long showId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetShowAwardsQuery(showId), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<ActionResult<ShowAwardDto>> AddAward(long showId, [FromBody] CreateShowAwardDto dto, CancellationToken cancellationToken)
    {
        var award = await _mediator.Send(new AddShowAwardCommand(showId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetAwards), new { showId }, award);
    }

    [HttpDelete("{awardId:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<IActionResult> RemoveAward(long showId, int awardId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveShowAwardCommand(showId, awardId), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}