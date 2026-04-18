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

    /// <summary>
    /// Gets the embedding vector for the current profile.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserVectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserVectorDto>> GetMyVector(CancellationToken cancellationToken)
    {
        var vector = await _mediator.Send(new GetUserVectorByProfileIdQuery(GetCurrentProfileId()), cancellationToken);
        if (vector == null) return NotFound();
        return Ok(vector);
    }

    /// <summary>
    /// Creates or updates the embedding vector for the current profile.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserVectorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserVectorDto>> UpsertMyVector([FromBody] CreateUpdateUserVectorDto dto, CancellationToken cancellationToken)
    {
        var vector = await _mediator.Send(new UpsertUserVectorCommand(GetCurrentProfileId(), dto), cancellationToken);
        return Ok(vector);
    }
}