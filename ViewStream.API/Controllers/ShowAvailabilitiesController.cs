using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability;
using ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability;
using ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ShowAvailability;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/shows/{showId:long}/availabilities")]
[Produces("application/json")]
public class ShowAvailabilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShowAvailabilitiesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ShowAvailabilityListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShowAvailabilityListItemDto>>> GetAvailabilitiesByShow(long showId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAvailabilitiesByShowQuery(showId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{countryCode}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShowAvailabilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShowAvailabilityDto>> GetAvailability(long showId, string countryCode, CancellationToken cancellationToken)
    {
        var availability = await _mediator.Send(new GetShowAvailabilityQuery(showId, countryCode), cancellationToken);
        if (availability == null) return NotFound();
        return Ok(availability);
    }

    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAvailability(long showId, [FromBody] CreateShowAvailabilityDto dto, CancellationToken cancellationToken)
    {
        if (showId != dto.ShowId)
            return BadRequest("Show ID mismatch.");

        var result = await _mediator.Send(new CreateShowAvailabilityCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetAvailability), new { showId = result.ShowId, countryCode = result.CountryCode }, result);
    }

    [HttpPut("{countryCode}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAvailability(long showId, string countryCode, [FromBody] UpdateShowAvailabilityDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateShowAvailabilityCommand(showId, countryCode, dto), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{countryCode}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAvailability(long showId, string countryCode, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteShowAvailabilityCommand(showId, countryCode), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}