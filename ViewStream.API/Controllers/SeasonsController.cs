using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Season.CreateSeason;
using ViewStream.Application.Commands.Season.DeleteSeason;
using ViewStream.Application.Commands.Season.RestoreSeason;
using ViewStream.Application.Commands.Season.UpdateSeason;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Season;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SeasonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeasonsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("show/{showId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SeasonListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SeasonListItemDto>>> GetSeasonsByShow(long showId, CancellationToken cancellationToken)
    {
        var seasons = await _mediator.Send(new GetSeasonsByShowQuery(showId), cancellationToken);
        return Ok(seasons);
    }

    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SeasonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeasonDto>> GetSeason(long id, CancellationToken cancellationToken)
    {
        var season = await _mediator.Send(new GetSeasonByIdQuery(id), cancellationToken);
        if (season == null) return NotFound();
        return Ok(season);
    }

    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSeason([FromBody] CreateSeasonDto dto, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var seasonId = await _mediator.Send(new CreateSeasonCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetSeason), new { id = seasonId }, seasonId);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSeason(long id, [FromBody] UpdateSeasonDto dto, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new UpdateSeasonCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSeason(long id, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteSeasonCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreSeason(long id, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new RestoreSeasonCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}