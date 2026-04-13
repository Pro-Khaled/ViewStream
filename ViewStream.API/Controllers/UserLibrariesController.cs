using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.UserLibrary.CreateUserLibrary;
using ViewStream.Application.Commands.UserLibrary.DeleteUserLibrary;
using ViewStream.Application.Commands.UserLibrary.UpdateUserLibrary;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserLibrary;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/library")]
[Authorize]
[Produces("application/json")]
public class UserLibrariesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserLibrariesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    /// <summary>
    /// Gets library summary for the current profile.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(UserLibrarySummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserLibrarySummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var summary = await _mediator.Send(new GetUserLibrarySummaryQuery(profileId), cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Gets paginated library items for the current profile.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserLibraryListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserLibraryListItemDto>>> GetLibrary(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new GetUserLibraryPagedQuery(profileId, page, pageSize, status), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Checks if a specific show or season is in the user's library.
    /// </summary>
    [HttpGet("check")]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserLibraryDto>> CheckLibrary(
        [FromQuery] long? showId,
        [FromQuery] long? seasonId,
        CancellationToken cancellationToken)
    {
        if (!showId.HasValue && !seasonId.HasValue)
            return BadRequest("Either showId or seasonId must be provided.");

        var profileId = GetCurrentProfileId();
        var item = await _mediator.Send(new GetUserLibraryByTargetQuery(profileId, showId, seasonId), cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Gets a specific library item by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserLibraryDto>> GetItem(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var item = await _mediator.Send(new GetUserLibraryByIdQuery(id, profileId), cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Adds a show or season to the user's library.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToLibrary([FromBody] CreateUserLibraryDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var item = await _mediator.Send(new CreateUserLibraryCommand(profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    /// <summary>
    /// Updates a library item (status, progress, score, etc.).
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(long id, [FromBody] UpdateUserLibraryDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var item = await _mediator.Send(new UpdateUserLibraryCommand(id, profileId, dto), cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Removes an item from the user's library.
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromLibrary(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new DeleteUserLibraryCommand(id, profileId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}