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

    /// <summary>
    /// Gets email preferences for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(EmailPreferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailPreferenceDto>> GetMyPreferences(CancellationToken cancellationToken)
    {
        var pref = await _mediator.Send(new GetEmailPreferenceQuery(GetCurrentUserId()), cancellationToken);
        if (pref == null) return NotFound();
        return Ok(pref);
    }

    /// <summary>
    /// Updates email preferences for the current user.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(EmailPreferenceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EmailPreferenceDto>> UpdatePreferences([FromBody] UpdateEmailPreferenceDto dto, CancellationToken cancellationToken)
    {
        var pref = await _mediator.Send(new UpdateEmailPreferenceCommand(GetCurrentUserId(), dto), cancellationToken);
        return Ok(pref);
    }
}