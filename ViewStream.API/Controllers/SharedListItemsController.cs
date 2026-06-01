using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.SharedListItem.AddShowToSharedList;
using ViewStream.Application.Commands.SharedListItem.DeleteSharedListItemAdmin;
using ViewStream.Application.Commands.SharedListItem.RemoveShowFromSharedList;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.SharedListItem;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/lists/{listId:long}/items")]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(SharedListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{showId:long}")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

[ApiController]
[Route("api/v1/admin/shared-list-items")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminSharedListItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSharedListItemsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Admin request body for adding a show to a shared list.
    /// </summary>
    public class CreateAdminSharedListItemDto
    {
        public long ListId { get; set; }
        public long ProfileId { get; set; }
        public long ShowId { get; set; }
    }

    /// <summary>
    /// Retrieves a paginated list of s-ha-re-dl-is-ti-te-ms for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of s-ha-re-dl-is-ti-te-ms.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminSharedListItemListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminSharedListItemListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminSharedListItemsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds a show to a shared list for the admin dashboard.
    /// </summary>
    /// <param name="dto">The shared list composite key and show to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created shared list item association.</returns>
    /// <response code="201">Show added successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="409">Show already exists in the list.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
    [ProducesResponseType(typeof(SharedListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SharedListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SharedListItemDto>> CreateAdminSharedListItem(
        [FromBody] CreateAdminSharedListItemDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.ListId <= 0) return BadRequest("ListId is required.");
        if (dto.ProfileId <= 0) return BadRequest("ProfileId is required.");
        if (dto.ShowId <= 0) return BadRequest("ShowId is required.");

        var actorUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        var addDto = new AddShowToSharedListDto
        {
            ShowId = dto.ShowId
        };

        var item = await _mediator.Send(
            new AddShowToSharedListCommand(dto.ListId, dto.ProfileId, addDto, actorUserId),
            cancellationToken);

        // Command returns the DTO; return 201 per requirement.
        return CreatedAtAction(nameof(GetAdminPaged), new { }, item);
    }

    /// <summary>
    /// Permanently deletes a shared list item record (hard delete). SuperAdmin only.
    /// </summary>
    /// <param name="id">The ID (ShowId) of the shared list item to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Shared list item hard-deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Item not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteSharedListItem(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _mediator.Send(new DeleteSharedListItemAdminCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
