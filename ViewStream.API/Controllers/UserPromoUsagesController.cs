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

    /// <summary>
    /// Gets all promo codes used by the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserPromoUsageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserPromoUsageDto>>> GetMyUsages(CancellationToken cancellationToken)
    {
        var usages = await _mediator.Send(new GetUserPromoUsagesQuery(GetCurrentUserId()), cancellationToken);
        return Ok(usages);
    }

    /// <summary>
    /// Redeems a promo code for the authenticated user.
    /// </summary>
    [HttpPost("redeem")]
    [ProducesResponseType(typeof(UserPromoUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserPromoUsageDto>> Redeem([FromBody] RedeemPromoCodeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var usage = await _mediator.Send(new RedeemPromoCodeCommand(GetCurrentUserId(), request.Code, request.PlanType), cancellationToken);
            return Ok(usage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

// Simple request DTO for the redeem endpoint
public class RedeemPromoCodeRequest
{
    public string Code { get; set; } = string.Empty;
    public string? PlanType { get; set; }
}