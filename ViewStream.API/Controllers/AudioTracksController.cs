using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.AudioTrack.CreateAudioTrack;
using ViewStream.Application.Commands.AudioTrack.DeleteAudioTrack;
using ViewStream.Application.Commands.AudioTrack.RestoreAudioTrack;
using ViewStream.Application.Commands.AudioTrack.UpdateAudioTrack;
using ViewStream.Application.Commands.AudioTrack.UploadAudioFile;
using ViewStream.Application.Common;
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

    /// <summary>
    /// Gets all audio tracks for a specific episode.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<AudioTrackListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AudioTrackListItemDto>>> GetAudioTracksByEpisode(long episodeId, CancellationToken cancellationToken)
    {
        var audioTracks = await _mediator.Send(new GetAudioTracksByEpisodeQuery(episodeId), cancellationToken);
        return Ok(audioTracks);
    }

    /// <summary>
    /// Gets a single audio track by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AudioTrackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AudioTrackDto>> GetAudioTrack(long id, CancellationToken cancellationToken)
    {
        var audioTrack = await _mediator.Send(new GetAudioTrackByIdQuery(id), cancellationToken);
        if (audioTrack == null) return NotFound();
        return Ok(audioTrack);
    }

    /// <summary>
    /// Creates a new audio track for an episode (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAudioTrack(long episodeId, [FromBody] CreateAudioTrackDto dto, CancellationToken cancellationToken)
    {
        if (episodeId != dto.EpisodeId)
            return BadRequest("Episode ID mismatch.");

        var id = await _mediator.Send(new CreateAudioTrackCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetAudioTrack), new { episodeId, id }, id);
    }

    /// <summary>
    /// Updates an existing audio track.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAudioTrack(long id, [FromBody] UpdateAudioTrackDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAudioTrackCommand(id, dto), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes an audio track.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAudioTrack(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAudioTrackCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted audio track (SuperAdmin only).
    /// </summary>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAudioTrack(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RestoreAudioTrackCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:long}/upload-file")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAudioFile(long id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var fileUrl = await _mediator.Send(new UploadAudioFileCommand(id, file, userId), cancellationToken);
        return Ok(new { audioUrl = fileUrl });
    }
}