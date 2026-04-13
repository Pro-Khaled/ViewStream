using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Rating.CreateRating;
using ViewStream.Application.Commands.Rating.DeleteRating;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Rating;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RatingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatingsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets the rating summary for a specific show.
    /// </summary>
    [HttpGet("show/{showId:long}/summary")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShowRatingSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShowRatingSummaryDto>> GetShowRatingSummary(long showId, CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetShowRatingSummaryQuery(showId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }

    /// <summary>
    /// Gets all ratings for a specific show.
    /// </summary>
    [HttpGet("show/{showId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<RatingListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RatingListItemDto>>> GetRatingsByShow(long showId, CancellationToken cancellationToken)
    {
        var ratings = await _mediator.Send(new GetRatingsByShowQuery(showId), cancellationToken);
        return Ok(ratings);
    }

    /// <summary>
    /// Gets the current user's rating for a specific show.
    /// </summary>
    [HttpGet("show/{showId:long}/me")]
    [Authorize]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RatingDto>> GetMyRating(long showId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId(); // You'll need to implement this based on your profile selection logic
        var rating = await _mediator.Send(new GetUserRatingForShowQuery(profileId, showId), cancellationToken);
        if (rating == null) return NotFound();
        return Ok(rating);
    }

    /// <summary>
    /// Gets all ratings made by the current user's profile.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(List<RatingListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RatingListItemDto>>> GetMyRatings(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var ratings = await _mediator.Send(new GetRatingsByProfileQuery(profileId), cancellationToken);
        return Ok(ratings);
    }

    /// <summary>
    /// Rates a show (creates or updates the user's rating).
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RatingDto>> RateShow([FromBody] CreateUpdateRatingDto dto, CancellationToken cancellationToken)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5.");

        var profileId = GetCurrentProfileId();
        var rating = await _mediator.Send(new UpsertRatingCommand(profileId, dto), cancellationToken);
        return Ok(rating);
    }

    /// <summary>
    /// Deletes the current user's rating for a show.
    /// </summary>
    [HttpDelete("show/{showId:long}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating(long showId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new DeleteRatingCommand(profileId, showId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    private long GetCurrentProfileId()
    {
        // TODO: Implement based on your profile selection mechanism (e.g., from claims, header, or query parameter)
        // For now, assuming profile ID is stored in a claim
        return long.Parse(User.FindFirstValue("ProfileId") ?? "0");
    }
}