using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.WatchParty.CreateWatchParty;
using ViewStream.Application.Commands.WatchParty.UpdateWatchParty;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.WatchParty;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Commands.WatchParty.EndWatchParty;
using ViewStream.Application.Commands.WatchParty.ForceCloseWatchParty;
using ViewStream.Application.Commands.WatchParty.DeleteWatchPartyAdmin;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/watch-parties")]
[Produces("application/json")]
public class WatchPartiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchPartiesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of active watch parties.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of active watch parties.</returns>
    /// <response code="200">Returns the paged list.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<WatchPartyListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<WatchPartyListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetActiveWatchPartiesPagedQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a watch party by its unique ID.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The watch party if found.</returns>
    /// <response code="200">Returns the watch party.</response>
    /// <response code="404">Party not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<WatchPartyDto>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var party = await _mediator.Send(new GetWatchPartyByIdQuery(id), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    /// <summary>
    /// Retrieves a watch party by its unique join code.
    /// </summary>
    /// <param name="code">The party code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active watch party if found.</returns>
    /// <response code="200">Returns the watch party.</response>
    /// <response code="404">Active party not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("code/{code}")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<WatchPartyDto>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var party = await _mediator.Send(new GetWatchPartyByCodeQuery(code), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new watch party for a specific episode.
    /// </summary>
    /// <param name="dto">The episode ID to watch together.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created watch party.</returns>
    /// <response code="201">Party created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<WatchPartyDto>> Create(
        [FromBody] CreateWatchPartyDto dto,
        CancellationToken cancellationToken)
    {
        var hostProfileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var party = await _mediator.Send(new CreateWatchPartyCommand(hostProfileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = party.Id }, party);
    }

    /// <summary>
    /// Updates a watch party (e.g., deactivate or set end time).
    /// Only the host can update.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="dto">The fields to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated party.</returns>
    /// <response code="200">Party updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the host.</response>
    /// <response code="404">Party not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(WatchPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<WatchPartyDto>> Update(
        long id,
        [FromBody] UpdateWatchPartyDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var party = await _mediator.Send(new UpdateWatchPartyCommand(id, profileId, dto, userId), cancellationToken);
        if (party == null) return NotFound();
        return Ok(party);
    }

    /// <summary>
    /// Ends a watch party (sets IsActive = false).
    /// Only the host can end the party.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Party ended successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the host.</response>
    /// <response code="404">Party not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/end")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> End(
        long id,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new EndWatchPartyCommand(id, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/watchparties")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminWatchPartiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminWatchPartiesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of watch parties for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="hostProfileId">Optional filter by host profile ID.</param>
    /// <param name="episodeId">Optional filter by episode ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of watch parties.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminWatchPartyListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminWatchPartyListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? hostProfileId = null,
        [FromQuery] long? episodeId = null,
        CancellationToken cancellationToken = default)
        {
            // GetAdminWatchPartiesPagedQuery ctor:
            // (pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, isActive, episodeId, hostProfileId)
            var query = new GetAdminWatchPartiesPagedQuery(
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                includeDeleted,
                isActive: null,
                episodeId: episodeId,
                hostProfileId: hostProfileId
            );

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Force closes (ends) an active watch party as administrator.
    /// </summary>
    /// <param name="id">The ID of the watch party to end.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id:long}/end")]
    [Authorize(Roles = "SuperAdmin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForceClose(long id, CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new ForceCloseWatchPartyCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Permanently or soft deletes a watch party as administrator.
    /// </summary>
    /// <param name="id">The ID of the watch party to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteWatchPartyAdminCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
