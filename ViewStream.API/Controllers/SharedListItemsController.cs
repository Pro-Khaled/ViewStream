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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all items in a shared list.
    /// </summary>
    /// <param name="listId">The ID of the shared list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of shows in the shared list.</returns>
    /// <response code="200">Returns the list of items.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SharedListItemListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SharedListItemListItemDto>>> GetItems(
        long listId,
        CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetItemsBySharedListQuery(listId), cancellationToken);
        return Ok(items);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Adds a show to a shared list.
    /// </summary>
    /// <param name="listId">The ID of the shared list.</param>
    /// <param name="dto">The show ID to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created list item.</returns>
    /// <response code="201">Show added successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to modify this list.</response>
    /// <response code="409">Show already exists in the list.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SharedListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddShow(
        long listId,
        [FromBody] AddShowToSharedListDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var item = await _mediator.Send(new AddShowToSharedListCommand(listId, profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetItems), new { listId }, item);
    }

    /// <summary>
    /// Removes a show from a shared list.
    /// </summary>
    /// <param name="listId">The ID of the shared list.</param>
    /// <param name="showId">The ID of the show to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Show removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to modify this list.</response>
    /// <response code="404">Item not found.</response>
    [HttpDelete("{showId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveShow(
        long listId,
        long showId,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RemoveShowFromSharedListCommand(listId, showId, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}