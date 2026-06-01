using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.SharedList.CreateSharedList;
using ViewStream.Application.Commands.SharedList.DeleteSharedList;
using ViewStream.Application.Commands.SharedList.GenerateShareCode;
using ViewStream.Application.Commands.SharedList.UpdateSharedList;
using ViewStream.Application.Commands.SharedList.RestoreSharedList;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.SharedList;
using ViewStream.Application.Commands.SharedList.DeleteSharedListAdmin;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/profiles/me/lists")]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("PublicReadRateLimit")]
    [ProducesResponseType(typeof(List<SharedListListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(SharedListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/share-code")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

[ApiController]
[Route("api/v1/admin/sharedlists")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminSharedListsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSharedListsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of shared lists for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of shared lists.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminSharedListListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminSharedListListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminSharedListsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Restores a soft-deleted shared list.
    /// </summary>
    /// <param name="id">Shared list id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Shared list restored.</response>
    /// <response code="404">Shared list not found or not deleted.</response>
    [HttpPost("{id:long}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreSharedList(
        long id,
        CancellationToken cancellationToken)
    {
        var actorUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var restored = await _mediator.Send(
            new RestoreSharedListCommand(id, actorUserId),
            cancellationToken);

        if (!restored)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific shared list as administrator.
    /// </summary>
    /// <param name="id">The ID of the shared list to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSharedList(long id, CancellationToken cancellationToken)
    {
        var adminUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteSharedListAdminCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
