using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Role.CreateRole;
using ViewStream.Application.Commands.Role.DeleteRole;
using ViewStream.Application.Commands.Role.UpdateRole;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Role;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/admin/roles")]
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
    /// Retrieves a role by ID with its assigned permissions.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested role.</returns>
    /// <response code="200">Returns the role.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Role not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RoleDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var role = await _mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return role == null ? NotFound() : Ok(role);
    }

    
        /// <summary>
        /// Retrieves a paginated list of roles for the admin dashboard.
        /// </summary>
        /// <param name="pageNumber">Page number (1-indexed).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of roles.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
        /// <response code="429">Too many requests. Please wait before trying again.</response>
        [HttpGet]
        [EnableRateLimiting("AdminRateLimit")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(typeof(PagedResult<AdminRoleListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<PagedResult<AdminRoleListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminRolesPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteRoleCommand(id, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}
