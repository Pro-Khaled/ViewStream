using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.Award.CreateAward;
using ViewStream.Application.Commands.Award.DeleteAward;
using ViewStream.Application.Commands.Award.UpdateAward;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Award;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AwardsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<List<AwardListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllAwardsQuery(), cancellationToken));

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<AwardListItemDto>>> GetPaged(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] int? year = null, CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new GetAwardsPagedQuery(page, pageSize, search, year), cancellationToken));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<AwardDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var award = await _mediator.Send(new GetAwardByIdQuery(id), cancellationToken);
        return award == null ? NotFound() : Ok(award);
    }

    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<ActionResult<AwardDto>> Create([FromBody] CreateAwardDto dto, CancellationToken cancellationToken)
    {
        var award = await _mediator.Send(new CreateAwardCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = award.Id }, award);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<ActionResult<AwardDto>> Update(int id, [FromBody] UpdateAwardDto dto, CancellationToken cancellationToken)
    {
        var award = await _mediator.Send(new UpdateAwardCommand(id, dto), cancellationToken);
        return award == null ? NotFound() : Ok(award);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAwardCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}