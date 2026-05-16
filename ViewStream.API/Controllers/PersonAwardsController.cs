using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.PersonAward.AddPersonAward;
using ViewStream.Application.Commands.PersonAward.RemovePersonAward;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PersonAward;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/persons/{personId:long}/awards")]
[Produces("application/json")]
public class PersonAwardsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PersonAwardsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all awards associated with a specific person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of awards for the person.</returns>
    /// <response code="200">Returns the list of awards.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PersonAwardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PersonAwardDto>>> GetAwards(
        long personId,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetPersonAwardsQuery(personId), cancellationToken));

    #endregion

    #region Commands

    /// <summary>
    /// Assigns an award to a person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="dto">The award details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created person-award association.</returns>
    /// <response code="201">Award assigned successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="409">Award already assigned to this person.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(PersonAwardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PersonAwardDto>> AddAward(
        long personId,
        [FromBody] CreatePersonAwardDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var award = await _mediator.Send(new AddPersonAwardCommand(personId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetAwards), new { personId }, award);
    }

    /// <summary>
    /// Removes an award assignment from a person.
    /// </summary>
    /// <param name="personId">The ID of the person.</param>
    /// <param name="awardId">The ID of the award to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Award removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Association not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{awardId:int}")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RemoveAward(
        long personId,
        int awardId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RemovePersonAwardCommand(personId, awardId, userId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/person-awards")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
[Produces("application/json")]
public class AdminPersonAwardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPersonAwardsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Admin request body for creating a person-award association.
    /// </summary>
    public class CreateAdminPersonAwardDto
    {
        public long PersonId { get; set; }
        public int AwardId { get; set; }
        public bool? Won { get; set; }
    }

    /// <summary>
    /// Retrieves a paginated list of p-er-so-na-wa-rd-s for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of p-er-so-na-wa-rd-s.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminPersonAwardListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminPersonAwardListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminPersonAwardsPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a person-award association for the admin dashboard.
    /// </summary>
    /// <param name="dto">The person-award composite key and award values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created person-award association.</returns>
    /// <response code="201">Person award created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="409">Association already exists.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ContentManager,Moderator")]
    [ProducesResponseType(typeof(PersonAwardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PersonAwardDto>> CreateAdminPersonAward(
        [FromBody] CreateAdminPersonAwardDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.PersonId <= 0) return BadRequest("PersonId is required.");
        if (dto.AwardId <= 0) return BadRequest("AwardId is required.");

        var actorUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        var createDto = new CreatePersonAwardDto
        {
            AwardId = dto.AwardId,
            Won = dto.Won
        };

        var award = await _mediator.Send(new AddPersonAwardCommand(dto.PersonId, createDto, actorUserId), cancellationToken);
        return Ok(award);
    }
}
