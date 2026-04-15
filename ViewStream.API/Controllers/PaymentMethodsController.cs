using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PaymentMethod.CreatePaymentMethod;
using ViewStream.Application.Commands.PaymentMethod.DeletePaymentMethod;
using ViewStream.Application.Commands.PaymentMethod.SetDefaultPaymentMethod;
using ViewStream.Application.Commands.PaymentMethod.UpdatePaymentMethod;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PaymentMethod;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentMethodsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Gets all payment methods belonging to the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentMethodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PaymentMethodDto>>> GetAll(CancellationToken cancellationToken)
    {
        var methods = await _mediator.Send(new GetUserPaymentMethodsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(methods);
    }

    /// <summary>
    /// Gets a specific payment method by ID (must belong to the user).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var method = await _mediator.Send(new GetPaymentMethodByIdQuery(id, GetCurrentUserId()), cancellationToken);
        if (method == null) return NotFound();
        return Ok(method);
    }

    /// <summary>
    /// Adds a new payment method for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentMethodDto>> Add([FromBody] CreatePaymentMethodDto dto, CancellationToken cancellationToken)
    {
        var method = await _mediator.Send(new AddPaymentMethodCommand(GetCurrentUserId(), dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = method.Id }, method);
    }

    /// <summary>
    /// Updates non-sensitive details of a payment method (e.g., expiry or default flag).
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodDto>> Update(long id, [FromBody] UpdatePaymentMethodDto dto, CancellationToken cancellationToken)
    {
        var method = await _mediator.Send(new UpdatePaymentMethodCommand(id, GetCurrentUserId(), dto), cancellationToken);
        if (method == null) return NotFound();
        return Ok(method);
    }

    /// <summary>
    /// Deletes a payment method permanently.
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePaymentMethodCommand(id, GetCurrentUserId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Sets the specified payment method as the default for the user.
    /// </summary>
    [HttpPost("{id:long}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefault(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SetDefaultPaymentMethodCommand(id, GetCurrentUserId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}