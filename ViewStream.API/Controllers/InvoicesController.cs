using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Invoice;

using ViewStream.Application.Queries.Invoice.GetInvoicePdf;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of invoices for the authenticated user.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of invoices.</returns>
    /// <response code="200">Returns the paginated invoices.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InvoiceListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<InvoiceListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUserInvoicesPagedQuery(GetCurrentUserId(), page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific invoice by ID.
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested invoice.</returns>
    /// <response code="200">Returns the invoice.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Invoice does not belong to the authenticated user.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id, GetCurrentUserId()), cancellationToken);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    /// <summary>
    /// Downloads a PDF version of a specific invoice.
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The PDF file content.</returns>
    /// <response code="200">Returns the PDF file.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Invoice does not belong to the authenticated user.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("{id:long}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInvoicePdfQuery(id, GetCurrentUserId()), cancellationToken);
        if (result == null) return NotFound();
        return File(result.Content, "application/pdf", result.FileName);
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/invoices")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Finance,Support")]
[Produces("application/json")]
public class AdminInvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminInvoicesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of invoices for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="userId">Optional filter by user ID.</param>
    /// <param name="status">Optional filter by status.</param>
    /// <param name="from">Optional filter by start date.</param>
    /// <param name="to">Optional filter by end date.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of invoices.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    //[HttpGet]
    //[ProducesResponseType(typeof(PagedResult<AdminInvoiceListItemDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status403Forbidden)]
    //[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    //public async Task<ActionResult<PagedResult<AdminInvoiceListItemDto>>> GetAdminPaged(
    //    [FromQuery] int pageNumber = 1,
    //    [FromQuery] int pageSize = 20,
    //    [FromQuery] long? userId = null,
    //    [FromQuery] string? status = null,
    //    [FromQuery] DateOnly? from = null,
    //    [FromQuery] DateOnly? to = null,
    //    [FromQuery] string sortBy = "invoiceDate",
    //    [FromQuery] bool sortDescending = true,
    //    CancellationToken cancellationToken = default)
    //{
    //    var query = new GetAdminInvoicesPagedQuery(pageNumber, pageSize, userId, status, from, to, sortBy, sortDescending);
    //    var result = await _mediator.Send(query, cancellationToken);
    //    return Ok(result);
    //}

    /// <summary>
    /// Retrieves a specific invoice by ID (Admin override).
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested invoice.</returns>
    /// <response code="200">Returns the invoice.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(long id, CancellationToken cancellationToken)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    /// <summary>
    /// Downloads a PDF version of a specific invoice (Admin override).
    /// </summary>
    /// <param name="id">The ID of the invoice.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The PDF file content.</returns>
    /// <response code="200">Returns the PDF file.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("{id:long}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdfAdmin(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInvoicePdfQuery(id), cancellationToken);
        if (result == null) return NotFound();
        return File(result.Content, "application/pdf", result.FileName);
    }
}
