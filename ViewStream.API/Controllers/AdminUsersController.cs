using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.User.AdminUpdateUser;
using ViewStream.Application.Commands.User.BlockUser;
using ViewStream.Application.Commands.User.DeleteUser;
using ViewStream.Application.Commands.User.UnblockUser;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.User;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "SuperAdmin,UserManager")]
[Produces("application/json")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries


    /// <summary>
    /// Retrieves a single user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User not found or deleted.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(long id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

    
        /// <summary>
        /// Retrieves a paginated list of users for the admin dashboard. Supports searching, sorting, and filtering by active/blocked status.
        /// </summary>
        /// <param name="pageNumber">Page number (1-indexed).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
        /// <param name="isActive">Optional filter by isactive.</param>
        /// <param name="isBlocked">Optional filter by isblocked.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of users.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
        [HttpGet("api/admin/users")]
        [Authorize(Roles = "SuperAdmin,UserManager")]
        [ProducesResponseType(typeof(PagedResult<AdminUserListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<AdminUserListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isBlocked = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminUsersPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, isActive, isBlocked);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
#endregion

    #region Commands

    /// <summary>
    /// Updates a user's profile (full name, phone, active status, roles).
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="dto">Fields to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User not found or deleted.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        long id,
        [FromBody] AdminUpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new AdminUpdateUserCommand(id, dto, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Blocks a user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="dto">Block reason and optional expiration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User blocked successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User not found or deleted.</response>
    [HttpPost("{id:long}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUser(
        long id,
        [FromBody] BlockUserDto dto,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new BlockUserCommand(id, dto, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Unblocks a user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User unblocked successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User not found or deleted.</response>
    [HttpPost("{id:long}/unblock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockUser(
        long id,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new UnblockUserCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft‑deletes a user (sets IsDeleted = true).
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User soft‑deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        long id,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteUserCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}
