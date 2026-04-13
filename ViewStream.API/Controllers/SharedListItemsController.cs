using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.SharedListItem.AddShowToSharedList;
using ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.SharedListItem;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/lists/{listId:long}/items")]
[Authorize]
[Produces("application/json")]
public class SharedListItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SharedListItemsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SharedListItemListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SharedListItemListItemDto>>> GetItems(long listId, CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetItemsBySharedListQuery(listId), cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SharedListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddShow(long listId, [FromBody] AddShowToSharedListDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var item = await _mediator.Send(new AddShowToSharedListCommand(listId, profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetItems), new { listId }, item);
    }

    [HttpDelete("{showId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveShow(long listId, long showId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new RemoveShowFromSharedListCommand(listId, showId, profileId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}