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

    /// <summary>
    /// Gets the currently active subscription for the authenticated user.
    /// </summary>
    [HttpGet("current")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionDto>> GetCurrent(CancellationToken cancellationToken)
    {
        var sub = await _mediator.Send(new GetCurrentSubscriptionQuery(GetCurrentUserId()), cancellationToken);
        if (sub == null) return NotFound();
        return Ok(sub);
    }

    /// <summary>
    /// Gets the complete subscription history for the authenticated user.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<SubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SubscriptionDto>>> GetHistory(CancellationToken cancellationToken)
    {
        var history = await _mediator.Send(new GetSubscriptionHistoryQuery(GetCurrentUserId()), cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Creates a new subscription for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionDto>> Create([FromBody] CreateSubscriptionDto dto, CancellationToken cancellationToken)
    {
        var sub = await _mediator.Send(new CreateSubscriptionCommand(GetCurrentUserId(), dto), cancellationToken);
        return CreatedAtAction(nameof(GetCurrent), null, sub);
    }

    /// <summary>
    /// Updates an existing subscription (e.g., change plan or payment method).
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionDto>> Update(long id, [FromBody] UpdateSubscriptionDto dto, CancellationToken cancellationToken)
    {
        var sub = await _mediator.Send(new UpdateSubscriptionCommand(id, dto), cancellationToken);
        if (sub == null) return NotFound();
        return Ok(sub);
    }

    /// <summary>
    /// Cancels an active subscription immediately.
    /// </summary>
    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSubscriptionCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}