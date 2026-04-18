using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.UserRole.AssignRoleToUser;
using ViewStream.Application.Commands.UserRole.RemoveRoleFromUser;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserRole;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/users/{userId:long}/roles")]
[Authorize(Roles = "SuperAdmin")]
[Produces("application/json")]
public class UserRolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserRolesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets all roles assigned to a user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserRoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserRoleDto>>> GetUserRoles(long userId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetUserRolesQuery(userId), cancellationToken));

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleDto>> AssignRole(long userId, [FromBody] AssignRoleToUserDto dto, CancellationToken cancellationToken)
    {
        var userRole = await _mediator.Send(new AssignRoleToUserCommand(userId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetUserRoles), new { userId }, userRole);
    }

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    [HttpDelete("{roleId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRole(long userId, long roleId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveRoleFromUserCommand(userId, roleId), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}