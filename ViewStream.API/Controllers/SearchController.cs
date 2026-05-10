using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ViewStream.Application.Commands.SearchLog.CreateSearchLog;
using ViewStream.Application.DTOs;

namespace ViewStream.Api.Controllers;

/// <summary>
/// Handles search-related actions including logging user queries.
/// </summary>
[ApiController]
[Route("api/v1/search")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator) => _mediator = mediator;

    private long? GetCurrentProfileId()
    {
        var claim = User.FindFirstValue("ProfileId");
        return claim != null && long.TryParse(claim, out var id) ? id : null;
    }

    /// <summary>
    /// Logs a search query. Anonymous users are accepted; authenticated users have their
    /// profile ID automatically recorded.
    /// </summary>
    /// <param name="dto">The search query and optional result metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created search log entry.</returns>
    /// <response code="201">Log entry created.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="429">Too many search requests. Slow down.</response>
    [HttpPost("log")]
    [AllowAnonymous]
    [EnableRateLimiting("SearchRateLimit")]
    [ProducesResponseType(typeof(SearchLogDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<SearchLogDto>> LogSearch(
        [FromBody] CreateSearchLogDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new CreateSearchLogCommand(profileId, dto), cancellationToken);
        return StatusCode(201, result);
    }
}
