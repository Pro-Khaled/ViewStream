using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Invoice;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Gets a paginated list of invoices for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InvoiceListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<InvoiceListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUserInvoicesPagedQuery(GetCurrentUserId(), page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific invoice by ID (must belong to the authenticated user).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id, GetCurrentUserId()), cancellationToken);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }
}
