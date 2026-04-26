using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ErrorLog;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/errors/logs")]
[Authorize(Roles = "SuperAdmin,Support")]
[Produces("application/json")]
public class AdminErrorLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminErrorLogsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of error logs with optional filters.
    /// </summary>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="errorCode">Optional filter by error code.</param>
    /// <param name="endpoint">Optional filter by endpoint path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of error logs (without stack traces).</returns>
    /// <response code="200">Returns the paginated error logs.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ErrorLogListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ErrorLogListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? errorCode = null,
        [FromQuery] string? endpoint = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetErrorLogsPagedQuery(page, pageSize, errorCode, endpoint), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single error log by ID, including the full stack trace and custom data.
    /// </summary>
    /// <param name="id">The error log ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed error information.</returns>
    /// <response code="200">Returns the error log.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Error log not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ErrorLogDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetErrorLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }
}