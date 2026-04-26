using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries / Validation

    /// <summary>
    /// Validates a promo code for applicability and calculates discount.
    /// </summary>
    /// <param name="dto">The promo code and optional plan type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation result with discount amount if valid.</returns>
    /// <response code="200">Validation completed successfully.</response>
    [HttpPost("validate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PromoCodeValidationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PromoCodeValidationResultDto>> Validate(
        [FromBody] ValidatePromoCodeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ValidatePromoCodeCommand(dto), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of promo codes.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="includeExpired">Whether to include expired promo codes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of promo codes.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PagedResult<PromoCodeListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    /// Retrieves a promo code by its code string.
    /// </summary>
    /// <param name="code">The promo code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The promo code details.</returns>
    /// <response code="200">Returns the promo code.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Promo code not found.</response>
    [HttpGet("code/{code}")]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeDto>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var promo = await _mediator.Send(new GetPromoCodeByCodeQuery(code), cancellationToken);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    /// <summary>
    /// Retrieves a promo code by ID.
    /// </summary>
    /// <param name="id">The ID of the promo code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The promo code details.</returns>
    /// <response code="200">Returns the promo code.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Promo code not found.</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var promo = await _mediator.Send(new GetPromoCodeByIdQuery(id), cancellationToken);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new promo code.
    /// </summary>
    /// <param name="dto">The promo code data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created promo code.</returns>
    /// <response code="201">Promo code created successfully.</response>
    /// <response code="400">Invalid input or duplicate code.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PromoCodeDto>> Create(
        [FromBody] CreatePromoCodeDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        try
        {
            var promo = await _mediator.Send(new CreatePromoCodeCommand(dto, userId), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = promo.Id }, promo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing promo code.
    /// </summary>
    /// <param name="id">The ID of the promo code to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated promo code.</returns>
    /// <response code="200">Promo code updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Promo code not found.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Marketing")]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PromoCodeDto>> Update(
        int id,
        [FromBody] UpdatePromoCodeDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var promo = await _mediator.Send(new UpdatePromoCodeCommand(id, dto, userId), cancellationToken);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    /// <summary>
    /// Deletes a promo code.
    /// </summary>
    /// <param name="id">The ID of the promo code to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Promo code deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Promo code not found.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeletePromoCodeCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}