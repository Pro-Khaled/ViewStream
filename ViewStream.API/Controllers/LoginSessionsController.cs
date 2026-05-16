using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions;
using ViewStream.Application.Commands.LoginSession.RevokeLoginSession;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.LoginSession;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/users/me/sessions")]
[Authorize]
[Produces("application/json")]
public class LoginSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginSessionsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all active login sessions for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active sessions with device information.</returns>
    /// <response code="200">Returns the list of active sessions.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<LoginSessionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<LoginSessionListItemDto>>> GetActiveSessions(CancellationToken cancellationToken)
    {
        var sessions = await _mediator.Send(new GetUserActiveSessionsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(sessions);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Revokes a specific login session, effectively logging out that device.
    /// </summary>
    /// <param name="id">The ID of the session to revoke.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Session revoked successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Session does not belong to the current user.</response>
    /// <response code="404">Session not found or already revoked.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("{id:long}/revoke")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Revoke(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RevokeLoginSessionCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Revokes all login sessions for the current user (global logout).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">All sessions revoked successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("revoke-all")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RevokeAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _mediator.Send(new RevokeAllUserSessionsCommand(userId, userId), cancellationToken);
        return NoContent();
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/loginsessions")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminLoginSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminLoginSessionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of login sessions for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of login sessions.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminLoginSessionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminLoginSessionListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminLoginSessionsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

