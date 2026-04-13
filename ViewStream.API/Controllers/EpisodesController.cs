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


    /// <summary>
    /// Gets all episodes for a specific season.
    /// </summary>
    [HttpGet("season/{seasonId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<EpisodeListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EpisodeListItemDto>>> GetEpisodesBySeason(long seasonId, CancellationToken cancellationToken)
    {
        var episodes = await _mediator.Send(new GetEpisodesBySeasonQuery(seasonId), cancellationToken);
        return Ok(episodes);
    }

    /// <summary>
    /// Gets a single episode by ID with full details.
    /// </summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EpisodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeDto>> GetEpisode(long id, CancellationToken cancellationToken)
    {
        var episode = await _mediator.Send(new GetEpisodeByIdQuery(id), cancellationToken);
        if (episode == null) return NotFound();
        return Ok(episode);
    }

    /// <summary>
    /// Creates a new episode (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEpisode([FromForm] CreateEpisodeDto dto, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var episodeId = await _mediator.Send(new CreateEpisodeCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetEpisode), new { id = episodeId }, episodeId);
    }


    /// <summary>
    /// Updates an existing episode.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEpisode(long id, [FromBody] UpdateEpisodeDto dto, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new UpdateEpisodeCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }


    /// <summary>
    /// Soft deletes an episode (also soft deletes related audio tracks and subtitles).
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEpisode(long id, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteEpisodeCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }


    /// <summary>
    /// Restores a soft-deleted episode.
    /// </summary>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreEpisode(long id, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new RestoreEpisodeCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }


    /// <summary>
    /// Uploads a thumbnail image for an episode.
    /// </summary>
    [HttpPost("{id:long}/upload-thumbnail")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadThumbnail(long id, IFormFile thumbnailFile, CancellationToken cancellationToken)
    {
        if (thumbnailFile == null || thumbnailFile.Length == 0)
            return BadRequest("No thumbnail file uploaded.");

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var thumbnailUrl = await _mediator.Send(new UploadEpisodeThumbnailCommand(id, thumbnailFile, userId), cancellationToken);
        return Ok(new { thumbnailUrl });
    }
}