using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.SearchLog;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/search/logs")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminSearchLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSearchLogsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets paginated search logs with optional filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SearchLogListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SearchLogListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] long? profileId = null,
        [FromQuery] string? query = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetSearchLogsPagedQuery(page, pageSize, profileId, query), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific search log by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SearchLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SearchLogDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetSearchLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }
}