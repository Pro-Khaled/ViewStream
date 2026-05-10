using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserInteraction;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/v1/admin/interactions")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminUserInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUserInteractionsController(IMediator mediator) => _mediator = mediator;

    #region Queries
    /// <summary>
    /// Retrieves a paginated list of user interactions for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="profileId">Optional filter by profileid.</param>
    /// <param name="showId">Optional filter by showid.</param>
    /// <param name="interactionType">Optional filter by interactiontype.</param>
    /// <param name="fromDate">Optional filter by fromdate.</param>
    /// <param name="toDate">Optional filter by todate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of userinteractions.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized â€“ authentication required.</response>
    /// <response code="403">Forbidden â€“ insufficient permissions.</response>
    [HttpGet("api/admin/interactions")]
    [Authorize(Roles = "SuperAdmin,Analytics")]
    [ProducesResponseType(typeof(PagedResult<AdminUserInteractionListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminUserInteractionListItemDto>>> GetAdminPaged(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? searchTerm = null,
    [FromQuery] string? sortBy = null,
    [FromQuery] bool sortDescending = false,
    [FromQuery] bool includeDeleted = false,
    [FromQuery] long? profileId = null,
    [FromQuery] long? showId = null,
    [FromQuery] string? interactionType = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserInteractionsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, profileId, showId, interactionType, fromDate, toDate);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    #endregion


}

/// <summary>
/// Nested admin interaction controllers:
///   GET /api/admin/profiles/{profileId}/interactions
///   GET /api/admin/profiles/{profileId}/interactions/summary
///   GET /api/admin/shows/{showId}/interactions
/// </summary>

// ──────────────────────────────────────────────────────────────────
// GET /api/admin/profiles/{profileId}/interactions
// GET /api/admin/profiles/{profileId}/interactions/summary
// ──────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/v1/admin/profiles/{profileId:long}/interactions")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminProfileInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminProfileInteractionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Retrieves a paginated list of user interactions for a specific profile.</summary>
    /// <param name="profileId">The ID of the profile.</param>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of interactions.</returns>
    /// <response code="200">Returns the paginated interactions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<UserInteractionListItemDto>>> GetByProfile(
        long profileId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInteractionsByProfileQuery(profileId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>Retrieves an interaction summary for a specific profile.</summary>
    /// <param name="profileId">The ID of the profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Interaction summary (counts by type, top shows).</returns>
    /// <response code="200">Returns the interaction summary.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Profile not found.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(UserInteractionSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInteractionSummaryDto>> GetProfileSummary(
        long profileId,
        CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetProfileInteractionSummaryQuery(profileId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }
}

// ──────────────────────────────────────────────────────────────────
// GET /api/admin/shows/{showId}/interactions
// ──────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/v1/admin/shows/{showId:long}/interactions")]
[Authorize(Roles = "SuperAdmin,Analytics")]
[Produces("application/json")]
public class AdminShowInteractionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminShowInteractionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Retrieves a paginated list of user interactions for a specific show.</summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of interactions.</returns>
    /// <response code="200">Returns the paginated interactions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserInteractionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<UserInteractionListItemDto>>> GetByShow(
        long showId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInteractionsByShowQuery(showId, page, pageSize), cancellationToken);
        return Ok(result);
    }
}


