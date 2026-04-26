using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Person.CreatePerson;
using ViewStream.Application.Commands.Person.DeletePerson;
using ViewStream.Application.Commands.Person.UpdatePerson;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Person;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all persons (useful for dropdowns).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all persons.</returns>
    /// <response code="200">Returns the list of persons.</response>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PersonListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PersonListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllPersonsQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a paginated list of persons.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="search">Optional search term to filter by name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of persons.</returns>
    /// <response code="200">Returns the paginated list.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<PersonListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PersonListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
        => Ok(await _mediator.Send(new GetPersonsPagedQuery(page, pageSize, search), cancellationToken));

    /// <summary>
    /// Retrieves a single person by ID.
    /// </summary>
    /// <param name="id">The ID of the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested person.</returns>
    /// <response code="200">Returns the person.</response>
    /// <response code="404">Person not found.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var person = await _mediator.Send(new GetPersonByIdQuery(id), cancellationToken);
        if (person == null) return NotFound();
        return Ok(person);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new person (actor, director, etc.).
    /// </summary>
    /// <param name="dto">The person data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created person.</returns>
    /// <response code="201">Person created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PersonDto>> Create(
        [FromBody] CreatePersonDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var person = await _mediator.Send(new CreatePersonCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="id">The ID of the person to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated person.</returns>
    /// <response code="200">Person updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Person not found.</response>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> Update(
        long id,
        [FromBody] UpdatePersonDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var person = await _mediator.Send(new UpdatePersonCommand(id, dto, userId), cancellationToken);
        if (person == null) return NotFound();
        return Ok(person);
    }

    /// <summary>
    /// Permanently deletes a person.
    /// </summary>
    /// <param name="id">The ID of the person to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Person deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Person not found.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeletePersonCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}