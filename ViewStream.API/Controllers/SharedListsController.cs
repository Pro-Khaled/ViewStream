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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all shared lists owned by the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of shared lists.</returns>
    /// <response code="200">Returns the list of shared lists.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SharedListListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<SharedListListItemDto>>> GetMyLists(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var lists = await _mediator.Send(new GetSharedListsByProfileQuery(profileId, true), cancellationToken);
        return Ok(lists);
    }

    /// <summary>
    /// Retrieves a specific shared list by ID.
    /// </summary>
    /// <param name="id">The ID of the shared list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested shared list.</returns>
    /// <response code="200">Returns the shared list.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">List does not belong to the current profile.</response>
    /// <response code="404">List not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SharedListDto>> GetList(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var list = await _mediator.Send(new GetSharedListByIdQuery(id, profileId), cancellationToken);
        if (list == null) return NotFound();
        return Ok(list);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new shared list.
    /// </summary>
    /// <param name="dto">The list data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created shared list.</returns>
    /// <response code="201">List created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateList(
        [FromBody] CreateSharedListDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var list = await _mediator.Send(new CreateSharedListCommand(profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetList), new { id = list.Id }, list);
    }

    /// <summary>
    /// Updates an existing shared list.
    /// </summary>
    /// <param name="id">The ID of the list to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated shared list.</returns>
    /// <response code="200">List updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">List does not belong to the current profile.</response>
    /// <response code="404">List not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateList(
        long id,
        [FromBody] UpdateSharedListDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var list = await _mediator.Send(new UpdateSharedListCommand(id, profileId, dto, userId), cancellationToken);
        if (list == null) return NotFound();
        return Ok(list);
    }

    /// <summary>
    /// Soft deletes a shared list.
    /// </summary>
    /// <param name="id">The ID of the list to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">List deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">List does not belong to the current profile.</response>
    /// <response code="404">List not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteList(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteSharedListCommand(id, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Generates a new share code for the specified list.
    /// </summary>
    /// <param name="id">The ID of the list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly generated share code.</returns>
    /// <response code="200">Share code generated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">List does not belong to the current profile.</response>
    /// <response code="404">List not found.</response>
    [HttpPost("{id:long}/share-code")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GenerateShareCode(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var shareCode = await _mediator.Send(new GenerateShareCodeCommand(id, profileId, userId), cancellationToken);
        if (shareCode == null) return NotFound();
        return Ok(new { shareCode });
    }

    #endregion
}