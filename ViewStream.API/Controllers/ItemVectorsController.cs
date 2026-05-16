using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ItemVector.UpsertItemVector;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ItemVector;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/shows/{showId:long}/vector")]
[Authorize(Roles = "ContentManager,SuperAdmin")]
[Produces("application/json")]
public class ItemVectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemVectorsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the embedding vector for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The embedding vector if it exists.</returns>
    /// <response code="200">Returns the embedding vector.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Vector not found for the specified show.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ItemVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemVectorDto>> GetVector(
        long showId,
        CancellationToken cancellationToken)
    {
        var vector = await _mediator.Send(new GetItemVectorByShowIdQuery(showId), cancellationToken);
        if (vector == null) return NotFound();
        return Ok(vector);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates or updates the embedding vector for a show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="dto">The embedding data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated vector.</returns>
    /// <response code="200">Vector upserted successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [ProducesResponseType(typeof(ItemVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ItemVectorDto>> UpsertVector(
        long showId,
        [FromBody] CreateUpdateItemVectorDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var vector = await _mediator.Send(new UpsertItemVectorCommand(showId, dto, userId), cancellationToken);
        return Ok(vector);
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/itemvectors")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager")]
[Produces("application/json")]
public class AdminItemVectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminItemVectorsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of item vectors for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="showId">Optional filter by show ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of item vectors.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminItemVectorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminItemVectorListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? showId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminItemVectorsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, showId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
