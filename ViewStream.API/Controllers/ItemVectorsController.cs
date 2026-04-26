using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ItemVector.UpsertItemVector;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ItemVector;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/shows/{showId:long}/vector")]
[Authorize(Roles = "ContentManager,SuperAdmin")]
[Produces("application/json")]
public class ItemVectorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemVectorsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the embedding vector for a specific show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The embedding vector if it exists.</returns>
    /// <response code="200">Returns the embedding vector.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Vector not found for the specified show.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ItemVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemVectorDto>> GetVector(
        long showId,
        CancellationToken cancellationToken)
    {
        var vector = await _mediator.Send(new GetItemVectorByShowIdQuery(showId), cancellationToken);
        if (vector == null) return NotFound();
        return Ok(vector);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates or updates the embedding vector for a show.
    /// </summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="dto">The embedding data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated vector.</returns>
    /// <response code="200">Vector upserted successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ItemVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ItemVectorDto>> UpsertVector(
        long showId,
        [FromBody] CreateUpdateItemVectorDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var vector = await _mediator.Send(new UpsertItemVectorCommand(showId, dto, userId), cancellationToken);
        return Ok(vector);
    }

    #endregion
}