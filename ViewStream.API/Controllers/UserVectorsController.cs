using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.UserVector.UpsertUserVector;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.UserVector;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/vector")]
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
    [HttpPost]
    [ProducesResponseType(typeof(UserVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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