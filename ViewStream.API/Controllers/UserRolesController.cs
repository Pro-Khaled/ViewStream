using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of role assignments.</returns>
    /// <response code="200">Returns the list of assigned roles.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserRoleDto>>> GetUserRoles(
        long userId,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetUserRolesQuery(userId), cancellationToken));

    #endregion

    #region Commands

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="dto">The role to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created role assignment.</returns>
    /// <response code="201">Role assigned successfully.</response>
    /// <response code="400">Invalid input or duplicate assignment.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserRoleDto>> AssignRole(
        long userId,
        [FromBody] AssignRoleToUserDto dto,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var userRole = await _mediator.Send(new AssignRoleToUserCommand(userId, dto, adminUserId), cancellationToken);
        return CreatedAtAction(nameof(GetUserRoles), new { userId }, userRole);
    }

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleId">The ID of the role to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Role removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User or role not found.</response>
    [HttpDelete("{roleId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRole(
        long userId,
        long roleId,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new RemoveRoleFromUserCommand(userId, roleId, adminUserId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}