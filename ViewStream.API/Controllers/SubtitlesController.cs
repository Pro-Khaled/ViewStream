using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Subtitle.CreateSubtitle;
using ViewStream.Application.Commands.Subtitle.DeleteSubtitle;
using ViewStream.Application.Commands.Subtitle.RestoreSubtitle;
using ViewStream.Application.Commands.Subtitle.UpdateSubtitle;
using ViewStream.Application.Commands.Subtitle.UploadSubtitleFile;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Subtitle;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/episodes/{episodeId:long}/subtitles")]
[Produces("application/json")]
public class SubtitlesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubtitlesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all subtitles for a specific episode.
    /// </summary>
    /// <param name="episodeId">The ID of the episode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of subtitles available for the episode.</returns>
    /// <response code="200">Returns the list of subtitles.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SubtitleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SubtitleListItemDto>>> GetSubtitlesByEpisode(
        long episodeId,
        CancellationToken cancellationToken)
    {
        var subtitles = await _mediator.Send(new GetSubtitlesByEpisodeQuery(episodeId), cancellationToken);
        return Ok(subtitles);
    }

    /// <summary>
    /// Retrieves a single subtitle by ID with full details.
    /// </summary>
    /// <param name="id">The ID of the subtitle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested subtitle.</returns>
    /// <response code="200">Returns the subtitle.</response>
    /// <response code="404">Subtitle not found.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SubtitleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubtitleDto>> GetSubtitle(
        long id,
        CancellationToken cancellationToken)
    {
        var subtitle = await _mediator.Send(new GetSubtitleByIdQuery(id), cancellationToken);
        if (subtitle == null) return NotFound();
        return Ok(subtitle);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new subtitle for an episode.
    /// </summary>
    /// <param name="episodeId">The ID of the episode (must match route).</param>
    /// <param name="dto">The subtitle data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created subtitle.</returns>
    /// <response code="201">Subtitle created successfully.</response>
    /// <response code="400">Episode ID mismatch or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSubtitle(
        long episodeId,
        [FromBody] CreateSubtitleDto dto,
        CancellationToken cancellationToken)
    {
        if (episodeId != dto.EpisodeId)
            return BadRequest("Episode ID mismatch.");

        var userId = GetCurrentUserId();
        var id = await _mediator.Send(new CreateSubtitleCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetSubtitle), new { episodeId, id }, id);
    }

    /// <summary>
    /// Updates an existing subtitle.
    /// </summary>
    /// <param name="id">The ID of the subtitle to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Subtitle updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Subtitle not found or already deleted.</response>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubtitle(
        long id,
        [FromBody] UpdateSubtitleDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateSubtitleCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a subtitle.
    /// </summary>
    /// <param name="id">The ID of the subtitle to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Subtitle deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Subtitle not found or already deleted.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubtitle(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteSubtitleCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Restores a soft‑deleted subtitle.
    /// </summary>
    /// <param name="id">The ID of the subtitle to restore.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Subtitle restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Subtitle not found or not deleted.</response>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreSubtitle(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RestoreSubtitleCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Uploads a subtitle file (e.g., .vtt, .srt) for an existing subtitle record.
    /// </summary>
    /// <param name="id">The ID of the subtitle record.</param>
    /// <param name="file">The subtitle file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded subtitle file.</returns>
    /// <response code="200">File uploaded successfully.</response>
    /// <response code="400">No file provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Subtitle not found.</response>
    [HttpPost("{id:long}/upload-file")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadSubtitleFile(
        long id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var userId = GetCurrentUserId();
        var fileUrl = await _mediator.Send(new UploadSubtitleFileCommand(id, file, userId), cancellationToken);
        return Ok(new { subtitleUrl = fileUrl });
    }

    #endregion
}