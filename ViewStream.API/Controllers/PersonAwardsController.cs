using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PersonAward.AddPersonAward;
using ViewStream.Application.Commands.PersonAward.RemovePersonAward;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PersonAward;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/persons/{personId:long}/awards")]
[Produces("application/json")]
public class PersonAwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PersonAwardsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all awards associated with a specific person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of awards for the person.</returns>
    /// <response code="200">Returns the list of awards.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PersonAwardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PersonAwardDto>>> GetAwards(
        long personId,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetPersonAwardsQuery(personId), cancellationToken));

    #endregion

    #region Commands

    /// <summary>
    /// Assigns an award to a person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="dto">The award details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created person-award association.</returns>
    /// <response code="201">Award assigned successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="409">Award already assigned to this person.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(PersonAwardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonAwardDto>> AddAward(
        long personId,
        [FromBody] CreatePersonAwardDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var award = await _mediator.Send(new AddPersonAwardCommand(personId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetAwards), new { personId }, award);
    }

    /// <summary>
    /// Removes an award assignment from a person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="awardId">The ID of the award to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Award removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Association not found.</response>
    [HttpDelete("{awardId:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAward(
        long personId,
        int awardId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RemovePersonAwardCommand(personId, awardId, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}