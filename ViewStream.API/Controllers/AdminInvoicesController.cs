using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Invoice;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/invoices")]
[Authorize(Roles = "SuperAdmin,Finance")]
[Produces("application/json")]
public class AdminInvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminInvoicesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a single invoice by ID. Only available to finance administrators.
    /// </summary>
    /// <param name="id">The invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice details.</returns>
    /// <response code="200">Returns the invoice.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have finance permissions.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }
}