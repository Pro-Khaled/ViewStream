using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all availability records for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of availability records with country and date details.</returns>
    /// <response code="200">Returns the list of availabilities.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ShowAvailabilityListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShowAvailabilityListItemDto>>> GetAvailabilitiesByShow(
        long showId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAvailabilitiesByShowQuery(showId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves availability details for a specific show in a specific country.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="countryCode">The ISO country code (e.g., "US").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The availability record if found.</returns>
    /// <response code="200">Returns the availability record.</response>
    /// <response code="404">Availability not found.</response>
    [HttpGet("{countryCode}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShowAvailabilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShowAvailabilityDto>> GetAvailability(
        long showId,
        string countryCode,
        CancellationToken cancellationToken)
    {
        var availability = await _mediator.Send(new GetShowAvailabilityQuery(showId, countryCode), cancellationToken);
        if (availability == null) return NotFound();
        return Ok(availability);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new availability record for a show in a specific country.
    /// </summary>
    /// <param name="showId">The ID of the show (must match the route).</param>
    /// <param name="dto">The availability data including country code and date range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created availability record.</returns>
    /// <response code="201">Availability created successfully.</response>
    /// <response code="400">Show ID mismatch or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(ShowAvailabilityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAvailability(
        long showId,
        [FromBody] CreateShowAvailabilityDto dto,
        CancellationToken cancellationToken)
    {
        if (showId != dto.ShowId)
            return BadRequest("Show ID mismatch.");

        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new CreateShowAvailabilityCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetAvailability),
            new { showId = result.ShowId, countryCode = result.CountryCode }, result);
    }

    /// <summary>
    /// Updates an existing availability record.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="countryCode">The ISO country code of the record to update.</param>
    /// <param name="dto">The updated date range and licensing notes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Availability updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Availability record not found.</response>
    [HttpPut("{countryCode}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAvailability(
        long showId,
        string countryCode,
        [FromBody] UpdateShowAvailabilityDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateShowAvailabilityCommand(showId, countryCode, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes an availability record (hard delete).
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="countryCode">The ISO country code to remove availability for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Availability deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Availability record not found.</response>
    [HttpDelete("{countryCode}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAvailability(
        long showId,
        string countryCode,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteShowAvailabilityCommand(showId, countryCode, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}