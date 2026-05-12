using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Rating.CreateRating;
using ViewStream.Application.Commands.Rating.DeleteRating;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Rating;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class RatingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatingsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all ratings made by the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of ratings.</returns>
    /// <response code="200">Returns the list of ratings.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("me")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(List<RatingListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<List<RatingListItemDto>>> GetMyRatings(CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var ratings = await _mediator.Send(new GetRatingsByProfileQuery(profileId), cancellationToken);
        return Ok(ratings);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Rates a show (creates or updates the current profile's rating).
    /// </summary>
    /// <param name="dto">The rating data (1-5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upserted rating.</returns>
    /// <response code="200">Rating saved successfully.</response>
    /// <response code="400">Rating must be between 1 and 5.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RatingDto>> RateShow(
        [FromBody] CreateUpdateRatingDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5.");

        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var rating = await _mediator.Send(new UpsertRatingCommand(profileId, dto, userId), cancellationToken);
        return Ok(rating);
    }

    #endregion
}

/// <summary>
/// Nested controller for Ratings under a specific Show.
/// GET    /api/Shows/{showId}/Ratings          → all ratings for the show
/// GET    /api/Shows/{showId}/Ratings/summary   → average + total count
/// GET    /api/Shows/{showId}/Ratings/me        → current profile's rating
/// DELETE /api/Shows/{showId}/Ratings/me        → remove current profile's rating
/// </summary>
[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/shows/{showId:long}/ratings")]
[Produces("application/json")]
public class ShowRatingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShowRatingsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>Retrieves all ratings for a specific show.</summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of ratings.</returns>
    /// <response code="200">Returns the list of ratings.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<RatingListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<List<RatingListItemDto>>> GetRatingsByShow(
        long showId, CancellationToken cancellationToken)
    {
        var ratings = await _mediator.Send(new GetRatingsByShowQuery(showId), cancellationToken);
        return Ok(ratings);
    }

    /// <summary>Retrieves the rating summary (average + count) for a specific show.</summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Average rating and total count.</returns>
    /// <response code="200">Returns the rating summary.</response>
    /// <response code="404">Show not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("summary")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShowRatingSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ShowRatingSummaryDto>> GetShowRatingSummary(
        long showId, CancellationToken cancellationToken)
    {
        var summary = await _mediator.Send(new GetShowRatingSummaryQuery(showId), cancellationToken);
        if (summary == null) return NotFound();
        return Ok(summary);
    }

    /// <summary>Retrieves the current profile's rating for a specific show.</summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's rating if it exists.</returns>
    /// <response code="200">Returns the rating.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">No rating found for this show.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("me")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RatingDto>> GetMyRating(
        long showId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var rating = await _mediator.Send(new GetUserRatingForShowQuery(profileId, showId), cancellationToken);
        if (rating == null) return NotFound();
        return Ok(rating);
    }

    #endregion

    #region Commands

    /// <summary>Deletes the current profile's rating for this show.</summary>
    /// <param name="showId">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Rating deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Rating not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("me")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteMyRating(
        long showId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteRatingCommand(profileId, showId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}
