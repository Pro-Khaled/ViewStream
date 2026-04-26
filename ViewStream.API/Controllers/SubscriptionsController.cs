using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Subscription.CreateSubscription;
using ViewStream.Application.Commands.Subscription.DeleteSubscription;
using ViewStream.Application.Commands.Subscription.UpdateSubscription;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Subscription;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the currently active subscription for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active subscription if one exists.</returns>
    /// <response code="200">Returns the active subscription.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">No active subscription found.</response>
    [HttpGet("current")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionDto>> GetCurrent(CancellationToken cancellationToken)
    {
        var sub = await _mediator.Send(new GetCurrentSubscriptionQuery(GetCurrentUserId()), cancellationToken);
        if (sub == null) return NotFound();
        return Ok(sub);
    }

    /// <summary>
    /// Retrieves the complete subscription history for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all past and current subscriptions.</returns>
    /// <response code="200">Returns the subscription history.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<SubscriptionDto>>> GetHistory(CancellationToken cancellationToken)
    {
        var history = await _mediator.Send(new GetSubscriptionHistoryQuery(GetCurrentUserId()), cancellationToken);
        return Ok(history);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new subscription for the authenticated user.
    /// </summary>
    /// <param name="dto">The subscription details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created subscription.</returns>
    /// <response code="201">Subscription created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SubscriptionDto>> Create(
        [FromBody] CreateSubscriptionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var sub = await _mediator.Send(new CreateSubscriptionCommand(userId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetCurrent), null, sub);
    }

    /// <summary>
    /// Updates an existing subscription (e.g., change plan or payment method).
    /// </summary>
    /// <param name="id">The ID of the subscription to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated subscription.</returns>
    /// <response code="200">Subscription updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Subscription not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionDto>> Update(
        long id,
        [FromBody] UpdateSubscriptionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var sub = await _mediator.Send(new UpdateSubscriptionCommand(id, dto, userId), cancellationToken);
        if (sub == null) return NotFound();
        return Ok(sub);
    }

    /// <summary>
    /// Cancels an active subscription immediately.
    /// </summary>
    /// <param name="id">The ID of the subscription to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Subscription cancelled successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Subscription not found.</response>
    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new CancelSubscriptionCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}