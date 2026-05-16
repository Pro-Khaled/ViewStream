using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ShowAvailability.CreateShowAvailability;
using ViewStream.Application.Commands.ShowAvailability.DeleteShowAvailability;
using ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ShowAvailability;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/shows/{showId:long}/availabilities")]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(ShowAvailabilityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{countryCode}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{countryCode}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

[ApiController]
[Route("api/v1/admin/show-availabilities")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminShowAvailabilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminShowAvailabilitiesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Admin request body for creating a show-availability association.
    /// </summary>
    public class CreateAdminShowAvailabilityDto
    {
        public long ShowId { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public DateOnly? AvailableFrom { get; set; }
        public DateOnly? AvailableUntil { get; set; }
        public string? LicensingNotes { get; set; }
    }

    /// <summary>
    /// Retrieves a paginated list of s-ho-wa-va-il-ab-il-it-ie-s for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of s-ho-wa-va-il-ab-il-it-ie-s.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminShowAvailabilityListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminShowAvailabilityListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminShowAvailabilitiesPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a show availability association for the admin dashboard.
    /// </summary>
    /// <param name="dto">The show availability composite key and values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created show availability association.</returns>
    /// <response code="201">Show availability created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="409">Association already exists.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
    [ProducesResponseType(typeof(ShowAvailabilityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ShowAvailabilityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ShowAvailabilityDto>> CreateAdminShowAvailability(
        [FromBody] CreateAdminShowAvailabilityDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.ShowId <= 0) return BadRequest("ShowId is required.");
        if (string.IsNullOrWhiteSpace(dto.CountryCode)) return BadRequest("CountryCode is required.");

        var actorUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        var createDto = new CreateShowAvailabilityDto
        {
            ShowId = dto.ShowId,
            CountryCode = dto.CountryCode,
            AvailableFrom = dto.AvailableFrom,
            AvailableUntil = dto.AvailableUntil,
            LicensingNotes = dto.LicensingNotes
        };

        var result = await _mediator.Send(new CreateShowAvailabilityCommand(createDto, actorUserId), cancellationToken);

        // CreateShowAvailabilityCommand returns (long ShowId, string CountryCode)
        // We currently don't have a command that returns the full DTO for this admin create.
        // Per requirements, return 201 Created with available identifier payload when DTO isn't available.
        return CreatedAtAction(
            nameof(GetAdminPaged),
            new { showId = result.ShowId },
            new ShowAvailabilityDto
            {
                ShowId = result.ShowId,
                CountryCode = result.CountryCode
            });
    }
}
