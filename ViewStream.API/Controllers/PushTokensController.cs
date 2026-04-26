using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PushToken.DeletePushToken;
using ViewStream.Application.Commands.PushToken.RegisterPushToken;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PushToken;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/users/me/push-tokens")]
[Authorize]
[Produces("application/json")]
public class PushTokensController : ControllerBase
{
    private readonly IMediator _mediator;

    public PushTokensController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all push notification tokens for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of push tokens with device information.</returns>
    /// <response code="200">Returns the list of push tokens.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PushTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PushTokenDto>>> GetMyTokens(CancellationToken cancellationToken)
    {
        var tokens = await _mediator.Send(new GetUserPushTokensQuery(GetCurrentUserId()), cancellationToken);
        return Ok(tokens);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Registers a new push notification token for the current user.
    /// </summary>
    /// <param name="dto">The device ID, token, and platform.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registered push token record.</returns>
    /// <response code="201">Token registered successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PushTokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PushTokenDto>> Register(
        [FromBody] CreatePushTokenDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var token = await _mediator.Send(new RegisterPushTokenCommand(userId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetMyTokens), null, token);
    }

    /// <summary>
    /// Deletes a push notification token belonging to the current user.
    /// </summary>
    /// <param name="id">The ID of the push token to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Token deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Token does not belong to the current user.</response>
    /// <response code="404">Token not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeletePushTokenCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}