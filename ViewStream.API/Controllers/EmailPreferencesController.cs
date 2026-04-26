using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.EmailPreference;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/users/me/email-preferences")]
[Authorize]
[Produces("application/json")]
public class EmailPreferencesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailPreferencesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the email preferences for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's email preferences.</returns>
    /// <response code="200">Returns the email preferences.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Email preferences not found (user may not have set them yet).</response>
    [HttpGet]
    [ProducesResponseType(typeof(EmailPreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailPreferenceDto>> GetMyPreferences(CancellationToken cancellationToken)
    {
        var pref = await _mediator.Send(new GetEmailPreferenceQuery(GetCurrentUserId()), cancellationToken);
        if (pref == null) return NotFound();
        return Ok(pref);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Updates the email preferences for the current user.
    /// </summary>
    /// <param name="dto">The updated email preference flags.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated email preferences.</returns>
    /// <response code="200">Preferences updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut]
    [ProducesResponseType(typeof(EmailPreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmailPreferenceDto>> UpdatePreferences(
        [FromBody] UpdateEmailPreferenceDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var pref = await _mediator.Send(new UpdateEmailPreferenceCommand(userId, dto, userId), cancellationToken);
        return Ok(pref);
    }

    #endregion
}