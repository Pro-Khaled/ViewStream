using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Episode.CreateEpisode;
using ViewStream.Application.Commands.Episode.DeleteEpisode;
using ViewStream.Application.Commands.Episode.RestoreEpisode;
using ViewStream.Application.Commands.Episode.UpdateEpisode;
using ViewStream.Application.Commands.Episode.UploadEpisodeThumbnail;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Episode;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class EpisodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EpisodesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a single episode by ID with full details.
    /// </summary>
    /// <param name="id">The ID of the episode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested episode.</returns>
    /// <response code="200">Returns the episode.</response>
    /// <response code="404">Episode not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EpisodeDto>> GetEpisode(
        long id,
        CancellationToken cancellationToken)
    {
        var episode = await _mediator.Send(new GetEpisodeByIdQuery(id), cancellationToken);
        if (episode == null) return NotFound();
        return Ok(episode);
    }

    /// <summary>
    /// Retrieves the stream URL for a single episode.
    /// </summary>
    /// <param name="id">The ID of the episode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream URL of the requested episode.</returns>
    /// <response code="200">Returns the stream URL.</response>
    /// <response code="404">Episode not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}/stream")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(EpisodeStreamUrlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EpisodeStreamUrlDto>> GetEpisodeStream(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEpisodeStreamQuery(id), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }
        
    #endregion

    #region Commands

    /// <summary>
    /// Creates a new episode.
    /// </summary>
    /// <param name="dto">Episode data including optional video file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created episode.</returns>
    /// <response code="201">Episode created successfully.</response>
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
    public async Task<IActionResult> CreateEpisode(
        [FromForm] CreateEpisodeDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var episodeId = await _mediator.Send(new CreateEpisodeCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetEpisode), new { id = episodeId }, episodeId);
    }

    /// <summary>
    /// Updates an existing episode.
    /// </summary>
    /// <param name="id">The ID of the episode to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Episode updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Episode not found or already deleted.</response>
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
    public async Task<IActionResult> UpdateEpisode(
        long id,
        [FromBody] UpdateEpisodeDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateEpisodeCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes an episode and its associated audio tracks/subtitles.
    /// </summary>
    /// <param name="id">The ID of the episode to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Episode deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Episode not found or already deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteEpisode(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteEpisodeCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Uploads a thumbnail image for an episode.
    /// </summary>
    /// <param name="id">The ID of the episode.</param>
    /// <param name="thumbnailFile">The thumbnail image file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded thumbnail.</returns>
    /// <response code="200">Thumbnail uploaded successfully.</response>
    /// <response code="400">No file provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Episode not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/upload-thumbnail")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> UploadThumbnail(
        long id,
        IFormFile thumbnailFile,
        CancellationToken cancellationToken)
    {
        if (thumbnailFile == null || thumbnailFile.Length == 0)
            return BadRequest("No thumbnail file uploaded.");

        var userId = GetCurrentUserId();
        var thumbnailUrl = await _mediator.Send(new UploadEpisodeThumbnailCommand(id, thumbnailFile, userId), cancellationToken);
        return Ok(new { thumbnailUrl });
    }

    #endregion
}

/// <summary>
/// Nested controller: GET /api/Seasons/{seasonId}/Episodes
/// Returns all episodes belonging to a specific season.
/// </summary>
[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/seasons/{seasonId:long}/episodes")]
[Produces("application/json")]
public class SeasonEpisodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeasonEpisodesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves all episodes belonging to a specific season.
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of episodes for the season.</returns>
    /// <response code="200">Returns the list of episodes.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<EpisodeListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<List<EpisodeListItemDto>>> GetEpisodesBySeason(
        long seasonId,
        CancellationToken cancellationToken)
    {
        var episodes = await _mediator.Send(new GetEpisodesBySeasonQuery(seasonId), cancellationToken);
        return Ok(episodes);
    }
}

[ApiController]
[Route("api/v1/admin/episodes")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminEpisodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminEpisodesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of episodes for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of episodes.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminEpisodeListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminEpisodeListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAdminEpisodesPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Restores a soft-deleted episode.
    /// </summary>
    /// <param name="id">The ID of the episode to restore.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Episode restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Episode not found or not deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RestoreEpisode(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new RestoreEpisodeCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
