using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Episode.CreateEpisode;
using ViewStream.Application.Commands.Episode.DeleteEpisode;
using ViewStream.Application.Commands.Episode.RestoreEpisode;
using ViewStream.Application.Commands.Episode.UpdateEpisode;
using ViewStream.Application.Commands.Episode.UploadEpisodeThumbnail;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Episode;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EpisodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EpisodesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all episodes belonging to a specific season.
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of episodes for the season.</returns>
    /// <response code="200">Returns the list of episodes.</response>
    [HttpGet("season/{seasonId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<EpisodeListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EpisodeListItemDto>>> GetEpisodesBySeason(
        long seasonId,
        CancellationToken cancellationToken)
    {
        var episodes = await _mediator.Send(new GetEpisodesBySeasonQuery(seasonId), cancellationToken);
        return Ok(episodes);
    }

    /// <summary>
    /// Retrieves a single episode by ID with full details.
    /// </summary>
    /// <param name="id">The ID of the episode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested episode.</returns>
    /// <response code="200">Returns the episode.</response>
    /// <response code="404">Episode not found.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> GetEpisode(
        long id,
        CancellationToken cancellationToken)
    {
        var episode = await _mediator.Send(new GetEpisodeByIdQuery(id), cancellationToken);
        if (episode == null) return NotFound();
        return Ok(episode);
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
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Restores a soft-deleted episode.
    /// </summary>
    /// <param name="id">The ID of the episode to restore.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Episode restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Episode not found or not deleted.</response>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreEpisode(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RestoreEpisodeCommand(id, userId), cancellationToken);
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
    [HttpPost("{id:long}/upload-thumbnail")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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