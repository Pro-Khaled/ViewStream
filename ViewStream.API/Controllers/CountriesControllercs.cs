using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Country.CreateCountry;
using ViewStream.Application.Commands.Country.DeleteCountry;
using ViewStream.Application.Commands.Country.UpdateCountry;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Country;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CountriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountriesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of countries.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="search">Optional search term to filter by country name or code.</param>
    /// <param name="continent">Optional filter by continent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of countries.</returns>
    /// <response code="200">Returns the paginated list of countries.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<CountryListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CountryListItemDto>>> GetCountries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null,
        [FromQuery] string? continent = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCountriesPagedQuery(page, pageSize, search, continent), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all countries (useful for dropdowns).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all countries.</returns>
    /// <response code="200">Returns the list of all countries.</response>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CountryListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CountryListItemDto>>> GetAllCountries(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCountriesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single country by its ISO code.
    /// </summary>
    /// <param name="code">The ISO country code (e.g., "US", "GB").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested country.</returns>
    /// <response code="200">Returns the country.</response>
    /// <response code="404">Country not found.</response>
    [HttpGet("{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CountryDto>> GetCountry(
        string code,
        CancellationToken cancellationToken)
    {
        var country = await _mediator.Send(new GetCountryByCodeQuery(code), cancellationToken);
        if (country == null) return NotFound();
        return Ok(country);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new country.
    /// </summary>
    /// <param name="dto">The data for the new country.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ISO code of the newly created country.</returns>
    /// <response code="201">Country created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="409">Country code already exists.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCountry(
        [FromBody] CreateCountryDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var code = await _mediator.Send(new CreateCountryCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetCountry), new { code }, code);
    }

    /// <summary>
    /// Updates an existing country.
    /// </summary>
    /// <param name="code">The ISO code of the country to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Country updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Country not found.</response>
    [HttpPut("{code}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCountry(
        string code,
        [FromBody] UpdateCountryDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateCountryCommand(code, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a country. This will cascade delete all related ShowAvailability records.
    /// </summary>
    /// <param name="code">The ISO code of the country to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Country deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Country not found.</response>
    [HttpDelete("{code}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCountry(
        string code,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteCountryCommand(code, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}