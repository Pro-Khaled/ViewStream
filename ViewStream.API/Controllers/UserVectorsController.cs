using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.UserVector.UpsertUserVector;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserVector;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/profiles/me/vector")]
[Authorize]
[Produces("application/json")]
public class UserVectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserVectorsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the embedding vector for the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user vector if it exists.</returns>
    /// <response code="200">Returns the embedding vector.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Vector not found for the current profile.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UserVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserVectorDto>> GetMyVector(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var vector = await _mediator.Send(new GetUserVectorByProfileIdQuery(profileId), cancellationToken);
        if (vector == null) return NotFound();
        return Ok(vector);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates or updates the embedding vector for the current profile.
    /// </summary>
    /// <param name="dto">The embedding data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upserted vector.</returns>
    /// <response code="200">Vector upserted successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [ProducesResponseType(typeof(UserVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserVectorDto>> UpsertMyVector(
        [FromBody] CreateUpdateUserVectorDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var vector = await _mediator.Send(new UpsertUserVectorCommand(profileId, dto, userId), cancellationToken);
        return Ok(vector);
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/uservectors")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager")]
[Produces("application/json")]
public class AdminUserVectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUserVectorsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of user vectors for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of user vectors.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminUserVectorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminUserVectorListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserVectorsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
