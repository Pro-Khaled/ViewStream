using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CountryListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CountryListItemDto>>> GetAllCountries(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCountriesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CountryDto>> GetCountry(string code, CancellationToken cancellationToken)
    {
        var country = await _mediator.Send(new GetCountryByCodeQuery(code), cancellationToken);
        if (country == null) return NotFound();
        return Ok(country);
    }

    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDto dto, CancellationToken cancellationToken)
    {
        var code = await _mediator.Send(new CreateCountryCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetCountry), new { code }, code);
    }

    [HttpPut("{code}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCountry(string code, [FromBody] UpdateCountryDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCountryCommand(code, dto), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{code}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCountry(string code, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCountryCommand(code), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}