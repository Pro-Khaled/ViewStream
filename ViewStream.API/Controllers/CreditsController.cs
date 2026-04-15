using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.Credit.CreateCredit;
using ViewStream.Application.Commands.Credit.DeleteCredit;
using ViewStream.Application.Commands.Credit.UpdateCredit;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Credit;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CreditsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreditsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets a paginated list of all credits (admin/global view).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CreditListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? role = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCreditsPagedQuery(page, pageSize, role), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single credit by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreditDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var credit = await _mediator.Send(new GetCreditByIdQuery(id), cancellationToken);
        if (credit == null) return NotFound();
        return Ok(credit);
    }

    /// <summary>
    /// Creates a new credit (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(CreditDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreditDto>> Create([FromBody] CreateCreditDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var credit = await _mediator.Send(new CreateCreditCommand(dto), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = credit.Id }, credit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing credit (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(CreditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreditDto>> Update(long id, [FromBody] UpdateCreditDto dto, CancellationToken cancellationToken)
    {
        var credit = await _mediator.Send(new UpdateCreditCommand(id, dto), cancellationToken);
        if (credit == null) return NotFound();
        return Ok(credit);
    }

    /// <summary>
    /// Deletes a credit (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCreditCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    // ----- Nested resource endpoints -----

    /// <summary>
    /// Gets all credits for a specific person.
    /// </summary>
    [HttpGet("by-person/{personId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetByPerson(long personId, CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsByPersonQuery(personId), cancellationToken);
        return Ok(credits);
    }

    /// <summary>
    /// Gets all credits for a specific show.
    /// </summary>
    [HttpGet("by-show/{showId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetByShow(long showId, CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsByShowQuery(showId), cancellationToken);
        return Ok(credits);
    }

    /// <summary>
    /// Gets all credits for a specific season.
    /// </summary>
    [HttpGet("by-season/{seasonId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetBySeason(long seasonId, CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsBySeasonQuery(seasonId), cancellationToken);
        return Ok(credits);
    }

    /// <summary>
    /// Gets all credits for a specific episode.
    /// </summary>
    [HttpGet("by-episode/{episodeId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetByEpisode(long episodeId, CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsByEpisodeQuery(episodeId), cancellationToken);
        return Ok(credits);
    }
}