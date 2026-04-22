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
    /// Gets paginated error logs with optional filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ErrorLogListItemDto>), StatusCodes.Status200OK)]
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
    /// Gets a specific error log by ID (includes full stack trace).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ErrorLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ErrorLogDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetErrorLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }
}