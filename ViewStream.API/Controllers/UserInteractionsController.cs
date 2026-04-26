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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves an interaction summary for the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A summary of interactions by type and top shows.</returns>
    /// <response code="200">Returns the interaction summary.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Profile not found.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(UserInteractionSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionSummaryDto>> GetMySummary(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var summary = await _mediator.Send(new GetProfileInteractionSummaryQuery(profileId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }

    /// <summary>
    /// Retrieves paginated interaction history for the current profile.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of interactions.</returns>
    /// <response code="200">Returns the paginated interactions.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// Retrieves a specific interaction by ID.
    /// </summary>
    /// <param name="id">The ID of the interaction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested interaction.</returns>
    /// <response code="200">Returns the interaction.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Interaction does not belong to the current profile.</response>
    /// <response code="404">Interaction not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UserInteractionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionDto>> GetInteraction(long id, CancellationToken cancellationToken)
    {
        var interaction = await _mediator.Send(new GetUserInteractionByIdQuery(id), cancellationToken);
        if (interaction == null || interaction.ProfileId != GetCurrentProfileId())
            return NotFound();
        return Ok(interaction);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Logs a user interaction (e.g., view, click, search).
    /// </summary>
    /// <param name="dto">The interaction details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recorded interaction.</returns>
    /// <response code="201">Interaction logged successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserInteractionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogInteraction(
        [FromBody] CreateUserInteractionDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var interaction = await _mediator.Send(new CreateUserInteractionCommand(profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetInteraction), new { id = interaction.Id }, interaction);
    }

    #endregion
}