using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.Role.CreateRole;
using ViewStream.Application.Commands.Role.DeleteRole;
using ViewStream.Application.Commands.Role.UpdateRole;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Role;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/roles")]
[Authorize(Roles = "SuperAdmin")]
[Produces("application/json")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets all roles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RoleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoleListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllRolesQuery(), cancellationToken));

    /// <summary>
    /// Gets a role by ID with its assigned permissions.
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var role = await _mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var role = await _mediator.Send(new CreateRoleCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Updates an existing role (system roles cannot be modified).
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> Update(long id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken)
    {
        var role = await _mediator.Send(new UpdateRoleCommand(id, dto), cancellationToken);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>
    /// Deletes a role (system roles cannot be deleted).
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}