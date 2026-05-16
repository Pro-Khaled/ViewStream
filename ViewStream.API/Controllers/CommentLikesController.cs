using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.CommentLike.CreateCommentLike;
using ViewStream.Application.Commands.CommentLike.DeleteCommentLike;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.CommentLike;
using Microsoft.AspNetCore.RateLimiting;
using ViewStream.Application.Common;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/comments/{commentId:long}/likes")]
[Produces("application/json")]
public class CommentLikesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentLikesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a summary of reactions for a specific comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Reaction summary including counts per reaction type and the current user's reaction (if authenticated).</returns>
    /// <response code="200">Returns the reaction summary.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("summary")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CommentReactionSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CommentReactionSummaryDto>> GetReactionSummary(
        long commentId,
        CancellationToken cancellationToken)
    {
        var profileId = User.Identity?.IsAuthenticated == true ? GetCurrentProfileId() : (long?)null;
        var summary = await _mediator.Send(new GetCommentReactionSummaryQuery(commentId, profileId), cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Retrieves all reactions for a specific comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of reactions on the comment.</returns>
    /// <response code="200">Returns the list of reactions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [EnableRateLimiting("PublicReadRateLimit")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CommentLikeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<List<CommentLikeDto>>> GetReactions(
        long commentId,
        CancellationToken cancellationToken)
    {
        var reactions = await _mediator.Send(new GetReactionsByCommentQuery(commentId), cancellationToken);
        return Ok(reactions);
    }

    /// <summary>
    /// Retrieves the current user's reaction on a specific comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's reaction, if present.</returns>
    /// <response code="200">Returns the reaction.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User has not reacted to this comment.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet("me")]
    [EnableRateLimiting("PublicReadRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CommentLikeDto>> GetMyReaction(
        long commentId,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var reaction = await _mediator.Send(new GetUserReactionForCommentQuery(commentId, profileId), cancellationToken);
        if (reaction == null) return NotFound();
        return Ok(reaction);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Adds or updates a reaction on a comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="dto">The reaction data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated reaction.</returns>
    /// <response code="200">Reaction saved successfully.</response>
    /// <response code="400">Comment ID mismatch or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CommentLikeDto>> React(
        long commentId,
        [FromBody] CreateUpdateCommentLikeDto dto,
        CancellationToken cancellationToken)
    {
        if (commentId != dto.CommentId)
            return BadRequest("Comment ID mismatch.");

        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var reaction = await _mediator.Send(new UpsertCommentLikeCommand(profileId, dto, userId), cancellationToken);
        return Ok(reaction);
    }

    /// <summary>
    /// Removes the current user's reaction from a comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Reaction removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">No reaction found to remove.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("me")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RemoveReaction(
        long commentId,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteCommentLikeCommand(commentId, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}

[ApiController]
[Route("api/v1/admin/commentlikes")]
[EnableRateLimiting("AdminRateLimit")]
[Authorize(Roles = "SuperAdmin,ContentManager")]
[Produces("application/json")]
public class AdminCommentLikesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCommentLikesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Admin request body for creating/upserting a comment like (composite key).
    /// </summary>
    public class CreateAdminCommentLikeDto
    {
        public long CommentId { get; set; }
        public long ProfileId { get; set; }
        public string? ReactionType { get; set; } = "like";
    }

    /// <summary>
    /// Retrieves a paginated list of comment likes for the admin dashboard.
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="sortBy">Optional field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <param name="includeDeleted">Whether to include soft-deleted records.</param>
    /// <param name="commentId">Optional filter by comment ID.</param>
    /// <param name="profileId">Optional filter by profile ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of comment likes.</returns>
    /// <response code="200">Returns the paginated list.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminCommentLikeListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResult<AdminCommentLikeListItemDto>>> GetAdminPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] long? commentId = null,
        [FromQuery] long? profileId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminCommentLikesPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted, commentId, profileId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a comment like (Admin override).
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="profileId">The ID of the profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Comment like deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="404">Comment like not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("{commentId:long}/{profileId:long}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteCommentLikeAdmin(
        long commentId,
        long profileId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ViewStream.Application.Commands.CommentLike.DeleteCommentLikeAdmin.DeleteCommentLikeAdminCommand(commentId, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Creates or updates a comment like for the admin dashboard (composite key).
    /// </summary>
    /// <param name="dto">The like data including composite keys and reaction type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated comment like.</returns>
    /// <response code="200">Comment like updated successfully.</response>
    /// <response code="201">Comment like created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,ContentManager")]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CommentLikeDto>> CreateAdminCommentLike(
        [FromBody] CreateAdminCommentLikeDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.CommentId <= 0) return BadRequest("CommentId is required.");
        if (dto.ProfileId <= 0) return BadRequest("ProfileId is required.");

        var actorUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        var createUpdateDto = new CreateUpdateCommentLikeDto
        {
            CommentId = dto.CommentId,
            ReactionType = dto.ReactionType
        };

        // UpsertCommentLikeCommand uses (ProfileId, CreateUpdateCommentLikeDto, UserId)
        var like = await _mediator.Send(
            new UpsertCommentLikeCommand(dto.ProfileId, createUpdateDto, actorUserId),
            cancellationToken);

        // Upsert does not reliably differentiate created vs updated; returning 200 OK is acceptable.
        return Ok(like);
    }
}
