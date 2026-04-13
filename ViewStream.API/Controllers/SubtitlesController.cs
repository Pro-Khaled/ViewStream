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

    /// <summary>
    /// Gets all subtitles for a specific episode.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SubtitleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SubtitleListItemDto>>> GetSubtitlesByEpisode(long episodeId, CancellationToken cancellationToken)
    {
        var subtitles = await _mediator.Send(new GetSubtitlesByEpisodeQuery(episodeId), cancellationToken);
        return Ok(subtitles);
    }

    /// <summary>
    /// Gets a single subtitle by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SubtitleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubtitleDto>> GetSubtitle(long id, CancellationToken cancellationToken)
    {
        var subtitle = await _mediator.Send(new GetSubtitleByIdQuery(id), cancellationToken);
        if (subtitle == null) return NotFound();
        return Ok(subtitle);
    }

    /// <summary>
    /// Creates a new subtitle for an episode (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubtitle(long episodeId, [FromBody] CreateSubtitleDto dto, CancellationToken cancellationToken)
    {
        if (episodeId != dto.EpisodeId)
            return BadRequest("Episode ID mismatch.");

        var id = await _mediator.Send(new CreateSubtitleCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetSubtitle), new { episodeId, id }, id);
    }

    /// <summary>
    /// Updates an existing subtitle.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubtitle(long id, [FromBody] UpdateSubtitleDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSubtitleCommand(id, dto), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a subtitle.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubtitle(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteSubtitleCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted subtitle (SuperAdmin only).
    /// </summary>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreSubtitle(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RestoreSubtitleCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }


    [HttpPost("{id:long}/upload-file")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadSubtitleFile(long id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var fileUrl = await _mediator.Send(new UploadSubtitleFileCommand(id, file, userId), cancellationToken);
        return Ok(new { subtitleUrl = fileUrl });
    }
}