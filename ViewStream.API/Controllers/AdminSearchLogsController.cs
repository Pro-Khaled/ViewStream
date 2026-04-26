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
    /// Retrieves a paginated list of search logs with optional filters.
    /// </summary>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="query">Optional filter by search term.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of search logs.</returns>
    /// <response code="200">Returns the paginated search logs.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SearchLogListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    /// Retrieves a single search log entry by its ID.
    /// </summary>
    /// <param name="id">The ID of the search log.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detailed search log entry.</returns>
    /// <response code="200">Returns the search log.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Search log not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SearchLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SearchLogDto>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetSearchLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }
}