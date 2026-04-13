using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.SharedList.CreateSharedList;
using ViewStream.Application.Commands.SharedList.DeleteSharedList;
using ViewStream.Application.Commands.SharedList.GenerateShareCode;
using ViewStream.Application.Commands.SharedList.UpdateSharedList;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.SharedList;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/lists")]
[Authorize]
[Produces("application/json")]
public class SharedListsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SharedListsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    [HttpGet]
    [ProducesResponseType(typeof(List<SharedListListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SharedListListItemDto>>> GetMyLists(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var lists = await _mediator.Send(new GetSharedListsByProfileQuery(profileId, true), cancellationToken);
        return Ok(lists);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateList([FromBody] CreateSharedListDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var list = await _mediator.Send(new CreateSharedListCommand(profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetList), new { id = list.Id }, list);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SharedListDto>> GetList(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var list = await _mediator.Send(new GetSharedListByIdQuery(id, profileId), cancellationToken);
        if (list == null) return NotFound();
        return Ok(list);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateList(long id, [FromBody] UpdateSharedListDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var list = await _mediator.Send(new UpdateSharedListCommand(id, profileId, dto), cancellationToken);
        if (list == null) return NotFound();
        return Ok(list);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteList(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new DeleteSharedListCommand(id, profileId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:long}/share-code")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GenerateShareCode(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var shareCode = await _mediator.Send(new GenerateShareCodeCommand(id, profileId), cancellationToken);
        if (shareCode == null) return NotFound();
        return Ok(new { shareCode });
    }
}