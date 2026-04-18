using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow;
using ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PersonalizedRow;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/recommendations")]
[Authorize]
[Produces("application/json")]
public class PersonalizedRowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonalizedRowsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    /// <summary>
    /// Gets all personalized recommendation rows for the current profile.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PersonalizedRowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PersonalizedRowDto>>> GetMyRows(CancellationToken cancellationToken)
    {
        var rows = await _mediator.Send(new GetPersonalizedRowsByProfileQuery(GetCurrentProfileId()), cancellationToken);
        return Ok(rows);
    }

    /// <summary>
    /// Creates or updates a personalized recommendation row (internal use).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PersonalizedRowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonalizedRowDto>> UpsertRow([FromBody] CreateUpdatePersonalizedRowDto dto, CancellationToken cancellationToken)
    {
        var row = await _mediator.Send(new UpsertPersonalizedRowCommand(GetCurrentProfileId(), dto), cancellationToken);
        return Ok(row);
    }

    /// <summary>
    /// Deletes a recommendation row for the current profile.
    /// </summary>
    [HttpDelete("{rowName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRow(string rowName, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePersonalizedRowCommand(GetCurrentProfileId(), rowName), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}