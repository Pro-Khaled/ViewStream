using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.UserInteraction.CreateUserInteraction;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserInteraction;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/interactions")]
[Authorize]
[Produces("application/json")]
public class UserInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserInteractionsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    /// <summary>
    /// Gets interaction summary for the current profile.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(UserInteractionSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserInteractionSummaryDto>> GetMySummary(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var summary = await _mediator.Send(new GetProfileInteractionSummaryQuery(profileId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }

    /// <summary>
    /// Gets paginated interaction history for the current profile.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserInteractionListItemDto>>> GetMyInteractions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new GetInteractionsByProfileQuery(profileId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Logs a user interaction (e.g., view, click, search).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserInteractionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> LogInteraction([FromBody] CreateUserInteractionDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var interaction = await _mediator.Send(new CreateUserInteractionCommand(profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetInteraction), new { id = interaction.Id }, interaction);
    }

    /// <summary>
    /// Gets a specific interaction by ID (must belong to current profile).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserInteractionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionDto>> GetInteraction(long id, CancellationToken cancellationToken)
    {
        var interaction = await _mediator.Send(new GetUserInteractionByIdQuery(id), cancellationToken);
        if (interaction == null || interaction.ProfileId != GetCurrentProfileId())
            return NotFound();
        return Ok(interaction);
    }
}