using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.WatchHistory.UpsertWatchHistory;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.WatchHistory;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/history")]
[Authorize]
[Produces("application/json")]
public class WatchHistoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public WatchHistoriesController(IMediator mediator) => _mediator = mediator;
    private long GetCurrentProfileId() => long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    [HttpGet("continue")]
    public async Task<ActionResult<List<WatchHistoryListItemDto>>> GetContinueWatching([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new GetContinueWatchingQuery(profileId, limit), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<WatchHistoryDto>> Upsert([FromBody] CreateUpdateWatchHistoryDto dto, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new UpsertWatchHistoryCommand(profileId, dto), cancellationToken);
        return Ok(result);
    }
}