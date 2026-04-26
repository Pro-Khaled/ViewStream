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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a library summary for the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Summary counts by status.</returns>
    /// <response code="200">Returns the library summary.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(UserLibrarySummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserLibrarySummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var summary = await _mediator.Send(new GetUserLibrarySummaryQuery(profileId), cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Retrieves paginated library items for the current profile.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="status">Optional filter by status (e.g., "watching", "completed").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of library items.</returns>
    /// <response code="200">Returns the library items.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserLibraryListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// Checks if a specific show or season is already in the current profile's library.
    /// </summary>
    /// <param name="showId">Optional show ID.</param>
    /// <param name="seasonId">Optional season ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The library entry if found.</returns>
    /// <response code="200">Returns the library entry.</response>
    /// <response code="400">Neither showId nor seasonId provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Not found in library.</response>
    [HttpGet("check")]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// Retrieves a specific library item by ID.
    /// </summary>
    /// <param name="id">The ID of the library entry.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested library entry.</returns>
    /// <response code="200">Returns the library entry.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Entry does not belong to the current profile.</response>
    /// <response code="404">Entry not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserLibraryDto>> GetItem(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var item = await _mediator.Send(new GetUserLibraryByIdQuery(id, profileId), cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Adds a show or season to the current profile's library.
    /// </summary>
    /// <param name="dto">The library entry details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created library entry.</returns>
    /// <response code="201">Library entry created successfully.</response>
    /// <response code="400">Invalid input (neither ShowId nor SeasonId provided).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="409">Item already exists in library.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddToLibrary(
        [FromBody] CreateUserLibraryDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        try
        {
            var item = await _mediator.Send(new CreateUserLibraryCommand(profileId, dto, userId), cancellationToken);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Updates a library item (status, progress, score, etc.).
    /// </summary>
    /// <param name="id">The ID of the library entry.</param>
    /// <param name="dto">The fields to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated library entry.</returns>
    /// <response code="200">Library entry updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Entry does not belong to the current profile.</response>
    /// <response code="404">Entry not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UserLibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(
        long id,
        [FromBody] UpdateUserLibraryDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var item = await _mediator.Send(new UpdateUserLibraryCommand(id, profileId, dto, userId), cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Removes an item from the current profile's library.
    /// </summary>
    /// <param name="id">The ID of the library entry to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Library entry removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Entry does not belong to the current profile.</response>
    /// <response code="404">Entry not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromLibrary(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteUserLibraryCommand(id, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}