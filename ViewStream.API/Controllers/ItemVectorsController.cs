using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    /// Gets the embedding vector for a specific show.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ItemVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemVectorDto>> GetVector(long showId, CancellationToken cancellationToken)
    {
        var vector = await _mediator.Send(new GetItemVectorByShowIdQuery(showId), cancellationToken);
        if (vector == null) return NotFound();
        return Ok(vector);
    }

    /// <summary>
    /// Creates or updates the embedding vector for a show.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ItemVectorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemVectorDto>> UpsertVector(long showId, [FromBody] CreateUpdateItemVectorDto dto, CancellationToken cancellationToken)
    {
        var vector = await _mediator.Send(new UpsertItemVectorCommand(showId, dto), cancellationToken);
        return Ok(vector);
    }
}