using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.PersonAward.AddPersonAward;
using ViewStream.Application.Commands.PersonAward.RemovePersonAward;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PersonAward;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/persons/{personId:long}/awards")]
[Produces("application/json")]
public class PersonAwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PersonAwardsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PersonAwardDto>>> GetAwards(long personId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetPersonAwardsQuery(personId), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<ActionResult<PersonAwardDto>> AddAward(long personId, [FromBody] CreatePersonAwardDto dto, CancellationToken cancellationToken)
    {
        var award = await _mediator.Send(new AddPersonAwardCommand(personId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetAwards), new { personId }, award);
    }

    [HttpDelete("{awardId:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<IActionResult> RemoveAward(long personId, int awardId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemovePersonAwardCommand(personId, awardId), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}