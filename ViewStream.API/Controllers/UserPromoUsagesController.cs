using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.UserPromoUsage.RedeemPromoCode;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserPromoUsage;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;


namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/users/me/promo-usages")]
[Authorize]
[Produces("application/json")]
public class UserPromoUsagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserPromoUsagesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all promo codes used by the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of used promo codes with redemption dates.</returns>
    /// <response code="200">Returns the list of used promo codes.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserPromoUsageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserPromoUsageDto>>> GetMyUsages(CancellationToken cancellationToken)
    {
        var usages = await _mediator.Send(new GetUserPromoUsagesQuery(GetCurrentUserId()), cancellationToken);
        return Ok(usages);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Redeems a promo code for the authenticated user.
    /// </summary>
    /// <param name="request">The promo code and optional plan type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created usage record if successful.</returns>
    /// <response code="200">Promo code redeemed successfully.</response>
    /// <response code="400">Invalid promo code, already used, or plan mismatch.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("redeem")]
    [EnableRateLimiting("ContentManagementRateLimit")]
    [ProducesResponseType(typeof(UserPromoUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserPromoUsageDto>> Redeem(
        [FromBody] RedeemPromoCodeRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        try
        {
            var usage = await _mediator.Send(new RedeemPromoCodeCommand(userId, request.Code, request.PlanType, userId), cancellationToken);
            return Ok(usage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
}

// Simple request DTO for the redeem endpoint
public class RedeemPromoCodeRequest
{
    public string Code { get; set; } = string.Empty;
    public string? PlanType { get; set; }
}

[ApiController]
[Route("api/v1/admin/userpromousages")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,Marketing")]
[Produces("application/json")]
public class AdminUserPromoUsagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUserPromoUsagesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of user promo usages for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="userId">Optional filter by user ID.</param>
    /// <param name="promoCodeId">Optional filter by promo code ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of user promo usages.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminUserPromoUsageListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminUserPromoUsageListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? userId = null,
        [FromQuery] int? promoCodeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminUserPromoUsagesPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, userId, promoCodeId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
