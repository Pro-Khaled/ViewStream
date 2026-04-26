using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.UserPromoUsage.RedeemPromoCode;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserPromoUsage;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/users/me/promo-usages")]
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
    [HttpPost("redeem")]
    [ProducesResponseType(typeof(UserPromoUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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