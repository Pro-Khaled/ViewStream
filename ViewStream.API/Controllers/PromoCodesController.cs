using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.PromoCode.CreatePromoCode;
using ViewStream.Application.Commands.PromoCode.DeletePromoCode;
using ViewStream.Application.Commands.PromoCode.UpdatePromoCode;
using ViewStream.Application.Commands.PromoCode.ValidatePromoCode;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.PromoCode;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PromoCodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromoCodesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Validates a promo code for applicability and calculates discount.
    /// </summary>
    [HttpPost("validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PromoCodeValidationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PromoCodeValidationResultDto>> Validate([FromBody] ValidatePromoCodeDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ValidatePromoCodeCommand(dto), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of promo codes (admin only).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PagedResult<PromoCodeListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PromoCodeListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeExpired = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPromoCodesPagedQuery(page, pageSize, includeExpired), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a promo code by its code string.
    /// </summary>
    [HttpGet("code/{code}")]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var promo = await _mediator.Send(new GetPromoCodeByCodeQuery(code), cancellationToken);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    /// <summary>
    /// Gets a promo code by ID (admin only).
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var promo = await _mediator.Send(new GetPromoCodeByIdQuery(id), cancellationToken);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    /// <summary>
    /// Creates a new promo code (admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PromoCodeDto>> Create([FromBody] CreatePromoCodeDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var promo = await _mediator.Send(new CreatePromoCodeCommand(dto), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = promo.Id }, promo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing promo code (admin only).
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeDto>> Update(int id, [FromBody] UpdatePromoCodeDto dto, CancellationToken cancellationToken)
    {
        var promo = await _mediator.Send(new UpdatePromoCodeCommand(id, dto), cancellationToken);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    /// <summary>
    /// Deletes a promo code (admin only).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePromoCodeCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}