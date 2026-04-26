using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.AudioTrack.CreateAudioTrack;
using ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack;
using ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack;
using ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack;
using ViewStream.Application.Commands.AudioTrack.UploadAudioFile;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.AudioTrack;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/episodes/{episodeId:long}/audio-tracks")]
[Produces("application/json")]
public class AudioTracksController : ControllerBase
{
    private readonly IMediator _mediator;

    public AudioTracksController(IMediator mediator) => _mediator = mediator;

    #region Queries

    /// <summary>
    /// Retrieves all audio tracks for a specific episode.
    /// </summary>
    /// <param name="episodeId">The ID of the episode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of audio tracks belonging to the episode.</returns>
    /// <response code="200">Returns the list of audio tracks.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<AudioTrackListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AudioTrackListItemDto>>> GetAudioTracksByEpisode(
        long episodeId,
        CancellationToken cancellationToken)
    {
        var audioTracks = await _mediator.Send(new GetAudioTracksByEpisodeQuery(episodeId), cancellationToken);
        return Ok(audioTracks);
    }

    /// <summary>
    /// Retrieves a single audio track by its ID.
    /// </summary>
    /// <param name="id">The ID of the audio track.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested audio track.</returns>
    /// <response code="200">Returns the audio track.</response>
    /// <response code="404">If the audio track is not found or has been deleted.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AudioTrackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudioTrackDto>> GetAudioTrack(
        long id,
        CancellationToken cancellationToken)
    {
        var audioTrack = await _mediator.Send(new GetAudioTrackByIdQuery(id), cancellationToken);
        if (audioTrack == null) return NotFound();
        return Ok(audioTrack);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new audio track for an episode.
    /// </summary>
    /// <param name="episodeId">The ID of the episode.</param>
    /// <param name="dto">The data for the new audio track.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created audio track.</returns>
    /// <response code="201">Audio track created successfully.</response>
    /// <response code="400">Episode ID mismatch or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAudioTrack(
        long episodeId,
        [FromBody] CreateAudioTrackDto dto,
        CancellationToken cancellationToken)
    {
        if (episodeId != dto.EpisodeId)
            return BadRequest("Episode ID mismatch.");

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await _mediator.Send(new CreateAudioTrackCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetAudioTrack), new { episodeId, id }, id);
    }

    /// <summary>
    /// Updates an existing audio track.
    /// </summary>
    /// <param name="id">The ID of the audio track to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Audio track updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Audio track not found or already deleted.</response>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAudioTrack(
        long id,
        [FromBody] UpdateAudioTrackDto dto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new UpdateAudioTrackCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes an audio track.
    /// </summary>
    /// <param name="id">The ID of the audio track to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Audio track deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Audio track not found or already deleted.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAudioTrack(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteAudioTrackCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted audio track.
    /// </summary>
    /// <param name="id">The ID of the audio track to restore.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Audio track restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Audio track not found or not deleted.</response>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAudioTrack(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new RestoreAudioTrackCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Uploads an audio file for a specific audio track.
    /// </summary>
    /// <param name="id">The ID of the audio track.</param>
    /// <param name="file">The audio file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded audio file.</returns>
    /// <response code="200">File uploaded successfully.</response>
    /// <response code="400">No file provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Audio track not found.</response>
    [HttpPost("{id:long}/upload-file")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadAudioFile(
        long id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var fileUrl = await _mediator.Send(new UploadAudioFileCommand(id, file, userId), cancellationToken);
        return Ok(new { audioUrl = fileUrl });
    }

    #endregion
}