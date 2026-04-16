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

    /// <summary>
    /// Gets all active login sessions for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<LoginSessionListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LoginSessionListItemDto>>> GetActiveSessions(CancellationToken cancellationToken)
    {
        var sessions = await _mediator.Send(new GetUserActiveSessionsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(sessions);
    }

    /// <summary>
    /// Revokes a specific login session (logs out that device).
    /// </summary>
    [HttpPost("{id:long}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RevokeLoginSessionCommand(id, GetCurrentUserId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Revokes all login sessions for the current user (global logout).
    /// </summary>
    [HttpPost("revoke-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokeAll(CancellationToken cancellationToken)
    {
        await _mediator.Send(new RevokeAllUserSessionsCommand(GetCurrentUserId()), cancellationToken);
        return NoContent();
    }
}