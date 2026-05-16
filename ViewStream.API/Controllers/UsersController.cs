using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.DataDeletionRequest.CreateDataDeletionRequest;
using ViewStream.Application.Commands.DataDeletionRequest.DeleteDataDeletionRequest;
using ViewStream.Application.Commands.DataDeletionRequest.UpdateDataDeletionRequest;
using ViewStream.Application.Commands.User.ChangePassword;
using ViewStream.Application.Commands.User.UpdateProfile;
using ViewStream.Application.Commands.User.AdminUpdateUser;
using ViewStream.Application.Commands.User.BlockUser;
using ViewStream.Application.Commands.User.DeleteUser;
using ViewStream.Application.Commands.User.UnblockUser;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.User;
using ViewStream.Application.Queries.DataDeletionRequest;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the profile of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or account disabled.</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var user = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Updates the current user's profile (full name, phone, country).
    /// </summary>
    /// <param name="dto">The fields to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Profile updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("me")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateProfileCommand(userId, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    /// <param name="dto">Current and new password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Password changed successfully.</response>
    /// <response code="400">Invalid input or password policy violation.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("change-password")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new ChangePasswordCommand(userId, dto, userId), cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return BadRequest(ModelState);
        }
        return NoContent();
    }

    #endregion

    #region Data Deletion Requests

    /// <summary>
    /// Submits a data deletion request for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created data deletion request.</returns>
    /// <response code="200">Request created or returned if already pending.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("me/data-deletion-request")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<DataDeletionRequestDto>> RequestDataDeletion(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new CreateDataDeletionRequestCommand(userId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancels a pending data deletion request.
    /// </summary>
    /// <param name="id">The ID of the data deletion request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Request cancelled successfully.</response>
    /// <response code="400">Request cannot be cancelled (not pending or wrong state).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("me/data-deletion-request/{id:long}")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CancelDataDeletion(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteDataDeletionRequestCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion

    #region User Search

    /// <summary>
    /// Searches active users by name or email. Safe public endpoint for friend discovery.
    /// </summary>
    /// <param name="q">Search query (min 2 chars).</param>
    /// <param name="limit">Maximum results to return (default 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching users with public fields only.</returns>
    /// <response code="200">Returns matching users.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<UserPublicSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserPublicSearchResultDto>>> SearchUsers(
        [FromQuery] string q,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new List<UserPublicSearchResultDto>());

        var results = await _mediator.Send(new SearchUsersQuery(q, limit), cancellationToken);
        return Ok(results);
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/users")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,UserManager")]
[Produces("application/json")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Retrieves a paginated list of users for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="isActive">Optional filter by active status.</param>
    /// <param name="isBlocked">Optional filter by blocked status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of users.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminUserListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

    /// <summary>
    /// Retrieves a single user by ID (Admin override).
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">User not found or deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserDto>> GetUser(long id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/unblock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// Soft-deletes a user (sets IsDeleted = true).
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User soft-deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">User not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteUser(
        long id,
        CancellationToken cancellationToken)
    {
        var adminUserId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteUserCommand(id, adminUserId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}

[ApiController]
[Route("api/v1/admin/datadeletionrequests")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,DataProtectionOfficer")]
[Produces("application/json")]
public class AdminDataDeletionRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminDataDeletionRequestsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Retrieves a paginated list of data deletion requests for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="status">Optional filter by status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of data deletion requests.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminDataDeletionRequestListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminDataDeletionRequestListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminDataDeletionRequestsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, status);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single data deletion request by ID (Admin override).
    /// </summary>
    /// <param name="id">The request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full request details including user email.</returns>
    /// <response code="200">Returns the request.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<DataDeletionRequestDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var req = await _mediator.Send(new GetDataDeletionRequestByIdQuery(id), cancellationToken);
        if (req == null) return NotFound();
        return Ok(req);
    }

    /// <summary>
    /// Updates the status or confirmation code of a data deletion request.
    /// </summary>
    /// <param name="id">The request ID.</param>
    /// <param name="dto">New status and/or confirmation code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated request.</returns>
    /// <response code="200">Request updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<DataDeletionRequestDto>> UpdateStatus(
        long id,
        [FromBody] UpdateDataDeletionRequestDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateDataDeletionRequestCommand(id, dto, userId), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
