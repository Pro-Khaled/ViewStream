using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Season.CreateSeason;
using ViewStream.Application.Commands.Season.DeleteSeason;
using ViewStream.Application.Commands.Season.RestoreSeason;
using ViewStream.Application.Commands.Season.UpdateSeason;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Season;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SeasonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeasonsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a single season by ID with full details.
    /// </summary>
    /// <param name="id">The ID of the season.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested season.</returns>
    /// <response code="200">Returns the season.</response>
    /// <response code="404">Season not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SeasonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SeasonDto>> GetSeason(
        long id,
        CancellationToken cancellationToken)
    {
        var season = await _mediator.Send(new GetSeasonByIdQuery(id), cancellationToken);
        if (season == null) return NotFound();
        return Ok(season);
    }

    
        /// <summary>
        /// Retrieves a paginated list of seasons for the admin dashboard.
        /// </summary>
        /// <param name="pageNumber">Page number (1-indexed).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
        /// <param name="showId">Optional filter by showid.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of seasons.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
        [HttpGet("api/admin/seasons")]
    [EnableRateLimiting("AdminRateLimit")]
        [Authorize(Roles = "SuperAdmin,ContentManager")]
        [ProducesResponseType(typeof(PagedResult<AdminSeasonListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<PagedResult<AdminSeasonListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? showId = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminSeasonsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, showId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    #endregion

    #region Commands

    /// <summary>
    /// Creates a new season.
    /// </summary>
    /// <param name="dto">The season data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created season.</returns>
    /// <response code="201">Season created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateSeason(
        [FromBody] CreateSeasonDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var seasonId = await _mediator.Send(new CreateSeasonCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetSeason), new { id = seasonId }, seasonId);
    }

    /// <summary>
    /// Updates an existing season.
    /// </summary>
    /// <param name="id">The ID of the season to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Season updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Season not found or already deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> UpdateSeason(
        long id,
        [FromBody] UpdateSeasonDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateSeasonCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a season and all its episodes.
    /// </summary>
    /// <param name="id">The ID of the season to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Season deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Season not found or already deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteSeason(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteSeasonCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted season.
    /// </summary>
    /// <param name="id">The ID of the season to restore.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Season restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Season not found or not deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/restore")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RestoreSeason(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RestoreSeasonCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}

/// <summary>
/// Nested controller: GET /api/Shows/{showId}/Seasons
/// Returns all seasons belonging to a specific show.
/// </summary>
[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/shows/{showId:long}/seasons")]
[Produces("application/json")]
public class ShowSeasonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShowSeasonsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves all seasons belonging to a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of seasons for the show.</returns>
    /// <response code="200">Returns the list of seasons.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SeasonListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<List<SeasonListItemDto>>> GetSeasonsByShow(
        long showId,
        CancellationToken cancellationToken)
    {
        var seasons = await _mediator.Send(new GetSeasonsByShowQuery(showId), cancellationToken);
        return Ok(seasons);
    }
}
