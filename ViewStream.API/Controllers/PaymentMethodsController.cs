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

    #region Queries

    /// <summary>
    /// Retrieves all payment methods belonging to the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of payment methods with masked sensitive data.</returns>
    /// <response code="200">Returns the list of payment methods.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentMethodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PaymentMethodDto>>> GetAll(CancellationToken cancellationToken)
    {
        var methods = await _mediator.Send(new GetUserPaymentMethodsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(methods);
    }

    /// <summary>
    /// Retrieves a specific payment method by ID.
    /// </summary>
    /// <param name="id">The ID of the payment method.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested payment method.</returns>
    /// <response code="200">Returns the payment method.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Payment method does not belong to the current user.</response>
    /// <response code="404">Payment method not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var method = await _mediator.Send(new GetPaymentMethodByIdQuery(id, GetCurrentUserId()), cancellationToken);
        if (method == null) return NotFound();
        return Ok(method);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Adds a new payment method for the authenticated user.
    /// </summary>
    /// <param name="dto">The payment method details (tokenized).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created payment method.</returns>
    /// <response code="201">Payment method added successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaymentMethodDto>> Add(
        [FromBody] CreatePaymentMethodDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var method = await _mediator.Send(new AddPaymentMethodCommand(userId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = method.Id }, method);
    }

    /// <summary>
    /// Updates non-sensitive details of a payment method (e.g., expiry or default flag).
    /// </summary>
    /// <param name="id">The ID of the payment method to update.</param>
    /// <param name="dto">The updated details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated payment method.</returns>
    /// <response code="200">Payment method updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Payment method does not belong to the current user.</response>
    /// <response code="404">Payment method not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodDto>> Update(
        long id,
        [FromBody] UpdatePaymentMethodDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var method = await _mediator.Send(new UpdatePaymentMethodCommand(id, userId, dto, userId), cancellationToken);
        if (method == null) return NotFound();
        return Ok(method);
    }

    /// <summary>
    /// Permanently deletes a payment method.
    /// </summary>
    /// <param name="id">The ID of the payment method to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Payment method deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Payment method does not belong to the current user.</response>
    /// <response code="404">Payment method not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeletePaymentMethodCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Sets the specified payment method as the default for the user.
    /// </summary>
    /// <param name="id">The ID of the payment method to set as default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Default payment method updated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Payment method does not belong to the current user.</response>
    /// <response code="404">Payment method not found.</response>
    [HttpPost("{id:long}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefault(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new SetDefaultPaymentMethodCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}