using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Invoice;


namespace ViewStream.Api.Controllers
{
    [ApiController]
    [Route("api/admin/invoices")]
    [Authorize(Roles = "SuperAdmin,Finance")]
    [Produces("application/json")]
    public class AdminInvoicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminInvoicesController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets any invoice by ID (admin only).
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetById(long id, CancellationToken cancellationToken)
        {
            var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }
    }
}