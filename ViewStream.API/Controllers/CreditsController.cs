using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of all credits (global admin view).
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="role">Optional filter by role (e.g., "actor", "director").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of credits.</returns>
    /// <response code="200">Returns the paginated list of credits.</response>
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
    /// Retrieves a single credit by ID.
    /// </summary>
    /// <param name="id">The ID of the credit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested credit.</returns>
    /// <response code="200">Returns the credit.</response>
    /// <response code="404">Credit not found.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreditDto>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var credit = await _mediator.Send(new GetCreditByIdQuery(id), cancellationToken);
        if (credit == null) return NotFound();
        return Ok(credit);
    }

    /// <summary>
    /// Retrieves all credits for a specific person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of credits for the person.</returns>
    /// <response code="200">Returns the list of credits.</response>
    [HttpGet("by-person/{personId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetByPerson(
        long personId,
        CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsByPersonQuery(personId), cancellationToken);
        return Ok(credits);
    }

    /// <summary>
    /// Retrieves all credits for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of credits for the show.</returns>
    /// <response code="200">Returns the list of credits.</response>
    [HttpGet("by-show/{showId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetByShow(
        long showId,
        CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsByShowQuery(showId), cancellationToken);
        return Ok(credits);
    }

    /// <summary>
    /// Retrieves all credits for a specific season.
    /// </summary>
    /// <param name="seasonId">The ID of the season.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of credits for the season.</returns>
    /// <response code="200">Returns the list of credits.</response>
    [HttpGet("by-season/{seasonId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetBySeason(
        long seasonId,
        CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsBySeasonQuery(seasonId), cancellationToken);
        return Ok(credits);
    }

    /// <summary>
    /// Retrieves all credits for a specific episode.
    /// </summary>
    /// <param name="episodeId">The ID of the episode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of credits for the episode.</returns>
    /// <response code="200">Returns the list of credits.</response>
    [HttpGet("by-episode/{episodeId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CreditListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CreditListItemDto>>> GetByEpisode(
        long episodeId,
        CancellationToken cancellationToken)
    {
        var credits = await _mediator.Send(new GetCreditsByEpisodeQuery(episodeId), cancellationToken);
        return Ok(credits);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new credit (actor, director, etc.) for a show, season, or episode.
    /// </summary>
    /// <param name="dto">The credit data. Exactly one target (ShowId, SeasonId, or EpisodeId) must be provided.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created credit.</returns>
    /// <response code="201">Credit created successfully.</response>
    /// <response code="400">Invalid input (e.g., multiple or no targets specified).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(CreditDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreditDto>> Create(
        [FromBody] CreateCreditDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        try
        {
            var credit = await _mediator.Send(new CreateCreditCommand(dto, userId), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = credit.Id }, credit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing credit.
    /// </summary>
    /// <param name="id">The ID of the credit to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated credit.</returns>
    /// <response code="200">Credit updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Credit not found.</response>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(CreditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreditDto>> Update(
        long id,
        [FromBody] UpdateCreditDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var credit = await _mediator.Send(new UpdateCreditCommand(id, dto, userId), cancellationToken);
        if (credit == null) return NotFound();
        return Ok(credit);
    }

    /// <summary>
    /// Permanently deletes a credit.
    /// </summary>
    /// <param name="id">The ID of the credit to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Credit deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Credit not found.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteCreditCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}