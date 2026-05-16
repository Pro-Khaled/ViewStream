using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PersonalizedRow.DeletePersonalizedRow;
using ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PersonalizedRow;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/profiles/me/recommendations")]
[Authorize]
[Produces("application/json")]
public class PersonalizedRowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonalizedRowsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all personalized recommendation rows for the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of recommendation rows with show IDs.</returns>
    /// <response code="200">Returns the list of recommendation rows.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PersonalizedRowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PersonalizedRowDto>>> GetMyRows(CancellationToken cancellationToken)
    {
        var rows = await _mediator.Send(new GetPersonalizedRowsByProfileQuery(GetCurrentProfileId()), cancellationToken);
        return Ok(rows);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates or updates a personalized recommendation row.
    /// </summary>
    /// <param name="dto">The row name and list of show IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upserted recommendation row.</returns>
    /// <response code="200">Row upserted successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [ProducesResponseType(typeof(PersonalizedRowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PersonalizedRowDto>> UpsertRow(
        [FromBody] CreateUpdatePersonalizedRowDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var row = await _mediator.Send(new UpsertPersonalizedRowCommand(profileId, dto, userId), cancellationToken);
        return Ok(row);
    }

    /// <summary>
    /// Deletes a recommendation row for the current profile.
    /// </summary>
    /// <param name="rowName">The name of the row to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Row deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Row not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{rowName}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteRow(
        string rowName,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeletePersonalizedRowCommand(profileId, rowName, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/personalized-rows")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminPersonalizedRowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPersonalizedRowsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of p-er-so-na-li-ze-dr-ow-s for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of p-er-so-na-li-ze-dr-ow-s.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminPersonalizedRowListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminPersonalizedRowListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminPersonalizedRowsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
