using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Invoice;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/admin/invoices")]
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
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<InvoiceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    /// <summary>
    /// Retrieves a paginated list of invoices with optional filters.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="userId">Filter by user ID.</param>
    /// <param name="status">Filter by status (e.g. paid, pending).</param>
    /// <param name="from">Filter invoices from this date (inclusive).</param>
    /// <param name="to">Filter invoices up to this date (inclusive).</param>
    /// <param name="orderBy">Sort field: invoiceDate (default), amount, status.</param>
    /// <param name="isDescending">Whether to sort descending (default: true).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of invoices.</returns>
    /// <response code="200">Returns the paged invoice list.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(PagedResult<InvoiceListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<InvoiceListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] long? userId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] string orderBy = "invoiceDate",
        [FromQuery] bool isDescending = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetInvoicesPagedQuery(page, pageSize, userId, status, from, to, orderBy, isDescending),
            cancellationToken);
        return Ok(result);
    }
}
