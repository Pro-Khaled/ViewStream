using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    /// <summary>Gets all persons (for dropdowns).</summary>
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PersonListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetAllPersonsQuery(), cancellationToken));
    }

    /// <summary>Gets paginated list of persons.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<PersonListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetPersonsPagedQuery(page, pageSize, search), cancellationToken));
    }

    /// <summary>Gets a person by ID.</summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<PersonDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var person = await _mediator.Send(new GetPersonByIdQuery(id), cancellationToken);
        if (person == null) return NotFound();
        return Ok(person);
    }

    /// <summary>Creates a new person (admin only).</summary>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto, CancellationToken cancellationToken)
    {
        var person = await _mediator.Send(new CreatePersonCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    /// <summary>Updates an existing person (admin only).</summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    public async Task<ActionResult<PersonDto>> Update(long id, [FromBody] UpdatePersonDto dto, CancellationToken cancellationToken)
    {
        var person = await _mediator.Send(new UpdatePersonCommand(id, dto), cancellationToken);
        if (person == null) return NotFound();
        return Ok(person);
    }

    /// <summary>Deletes a person (admin only).</summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePersonCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}