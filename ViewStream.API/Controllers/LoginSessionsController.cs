using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.LoginSession.RevokeAllUserSessions;
using ViewStream.Application.Commands.LoginSession.RevokeLoginSession;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.LoginSession;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/users/me/sessions")]
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
    [HttpPost("{id:long}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [HttpPost("revoke-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _mediator.Send(new RevokeAllUserSessionsCommand(userId, userId), cancellationToken);
        return NoContent();
    }

    #endregion
}