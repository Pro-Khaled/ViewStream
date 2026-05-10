using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.DataDeletionRequest.UpdateDataDeletionRequest;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.DataDeletionRequest;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/data-deletion-requests")]
[Authorize(Roles = "SuperAdmin,DataProtectionOfficer")]
[Produces("application/json")]
public class AdminDataDeletionRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminDataDeletionRequestsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of data deletion requests.
    /// </summary>
    /// <param name="page">Page number (1‑indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="status">Optional filter by status (e.g., "pending", "completed").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of data deletion requests.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DataDeletionRequestListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<DataDeletionRequestListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetDataDeletionRequestsPagedQuery(page, pageSize, status), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single data deletion request by ID.
    /// </summary>
    /// <param name="id">The request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full request details including user email.</returns>
    /// <response code="200">Returns the request.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Request not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DataDeletionRequestDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var req = await _mediator.Send(new GetDataDeletionRequestByIdQuery(id), cancellationToken);
        if (req == null) return NotFound();
        return Ok(req);
    }

    
        /// <summary>
        /// Retrieves a paginated list of data deletion requests for the admin dashboard.
        /// </summary>
        /// <param name="pageNumber">Page number (1-indexed).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="sortBy">Optional field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
        /// <param name="status">Optional filter by status.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of datadeletionrequests.</returns>
        /// <response code="200">Returns the paginated list.</response>
        /// <response code="401">Unauthorized â€“ authentication required.</response>
        /// <response code="403">Forbidden â€“ insufficient permissions.</response>
        [HttpGet("api/admin/data-deletion-requests")]
        [Authorize(Roles = "SuperAdmin,DataProtectionOfficer")]
        [ProducesResponseType(typeof(PagedResult<AdminDataDeletionRequestListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<AdminDataDeletionRequestListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] string? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminDataDeletionRequestsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, status);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    #endregion

    #region Commands

    /// <summary>
    /// Updates the status or confirmation code of a data deletion request.
    /// </summary>
    /// <param name="id">The request ID.</param>
    /// <param name="dto">New status and/or confirmation code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated request.</returns>
    /// <response code="200">Request updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Request not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DataDeletionRequestDto>> UpdateStatus(
        long id,
        [FromBody] UpdateDataDeletionRequestDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateDataDeletionRequestCommand(id, dto, userId), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    #endregion
}
