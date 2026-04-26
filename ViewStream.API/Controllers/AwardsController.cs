using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Award.CreateAward;
using ViewStream.Application.Commands.Award.DeleteAward;
using ViewStream.Application.Commands.Award.UpdateAward;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Award;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AwardsController(IMediator mediator) => _mediator = mediator;

    #region Queries

    /// <summary>
    /// Retrieves all awards.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of awards.</returns>
    /// <response code="200">Returns the list of awards.</response>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<AwardListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AwardListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllAwardsQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a paged list of awards.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="search">Optional search term.</param>
    /// <param name="year">Optional year filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged result of awards.</returns>
    /// <response code="200">Returns the paged result.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<AwardListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AwardListItemDto>>> GetPaged(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] int? year = null, CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new GetAwardsPagedQuery(page, pageSize, search, year), cancellationToken));

    /// <summary>
    /// Retrieves a single award by its ID.
    /// </summary>
    /// <param name="id">The ID of the award.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested award.</returns>
    /// <response code="200">Returns the award.</response>
    /// <response code="404">If the award is not found.</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AwardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AwardDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var award = await _mediator.Send(new GetAwardByIdQuery(id), cancellationToken);
        return award == null ? NotFound() : Ok(award);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new award.
    /// </summary>
    /// <param name="dto">The data for the new award.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created award.</returns>
    /// <response code="201">Award created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(AwardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateAwardDto dto, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var award = await _mediator.Send(new CreateAwardCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = award.Id }, award);
    }

    /// <summary>
    /// Updates an existing award.
    /// </summary>
    /// <param name="id">The ID of the award to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated award.</returns>
    /// <response code="200">Award updated successfully.</response>
    /// <response code="404">Award not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(AwardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AwardDto>> Update(int id, [FromBody] UpdateAwardDto dto, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var award = await _mediator.Send(new UpdateAwardCommand(id, dto, userId), cancellationToken);
        return award == null ? NotFound() : Ok(award);
    }

    /// <summary>
    /// Deletes an award.
    /// </summary>
    /// <param name="id">The ID of the award to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Award deleted successfully.</response>
    /// <response code="404">Award not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new DeleteAwardCommand(id, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}