using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Permission.CreatePermission;
using ViewStream.Application.Commands.Permission.DeletePermission;
using ViewStream.Application.Commands.Permission.UpdatePermission;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Permission;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/permissions")]
[Authorize(Roles = "SuperAdmin")]
[Produces("application/json")]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all permissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all permissions.</returns>
    /// <response code="200">Returns the list of permissions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PermissionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PermissionListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllPermissionsQuery(), cancellationToken));

    /// <summary>
    /// Retrieves all permissions grouped by GroupName.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of permissions grouped by group name.</returns>
    /// <response code="200">Returns the grouped permissions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(Dictionary<string, List<PermissionListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Dictionary<string, List<PermissionListItemDto>>>> GetGrouped(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetPermissionsByGroupQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a single permission by ID.
    /// </summary>
    /// <param name="id">The ID of the permission.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested permission.</returns>
    /// <response code="200">Returns the permission.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Permission not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var perm = await _mediator.Send(new GetPermissionByIdQuery(id), cancellationToken);
        return perm == null ? NotFound() : Ok(perm);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="dto">The permission data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created permission.</returns>
    /// <response code="201">Permission created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var perm = await _mediator.Send(new CreatePermissionCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = perm.Id }, perm);
    }

    /// <summary>
    /// Updates an existing permission.
    /// </summary>
    /// <param name="id">The ID of the permission to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated permission.</returns>
    /// <response code="200">Permission updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Permission not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> Update(int id, [FromBody] UpdatePermissionDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var perm = await _mediator.Send(new UpdatePermissionCommand(id, dto, userId), cancellationToken);
        return perm == null ? NotFound() : Ok(perm);
    }

    /// <summary>
    /// Deletes a permission.
    /// </summary>
    /// <param name="id">The ID of the permission to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Permission deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Permission not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeletePermissionCommand(id, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}