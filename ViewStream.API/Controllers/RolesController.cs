using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all roles.</returns>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<RoleListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<RoleListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllRolesQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a role by ID with its assigned permissions.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested role.</returns>
    /// <response code="200">Returns the role.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Role not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var role = await _mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return role == null ? NotFound() : Ok(role);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="dto">The role data including optional permission IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created role.</returns>
    /// <response code="201">Role created successfully.</response>
    /// <response code="400">Invalid input or duplicate role name.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RoleDto>> Create(
        [FromBody] CreateRoleDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var role = await _mediator.Send(new CreateRoleCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Updates an existing role (system roles cannot be modified).
    /// </summary>
    /// <param name="id">The ID of the role to update.</param>
    /// <param name="dto">The updated description and permission IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated role.</returns>
    /// <response code="200">Role updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission or role is a system role.</response>
    /// <response code="404">Role not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> Update(
        long id,
        [FromBody] UpdateRoleDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var role = await _mediator.Send(new UpdateRoleCommand(id, dto, userId), cancellationToken);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>
    /// Deletes a role (system roles cannot be deleted).
    /// </summary>
    /// <param name="id">The ID of the role to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Role deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission or role is a system role.</response>
    /// <response code="404">Role not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteRoleCommand(id, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}