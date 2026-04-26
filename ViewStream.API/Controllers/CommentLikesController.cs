using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.CommentLike.CreateCommentLike;
using ViewStream.Application.Commands.CommentLike.DeleteCommentLike;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.CommentLike;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/comments/{commentId:long}/likes")]
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
    [HttpGet("summary")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CommentReactionSummaryDto), StatusCodes.Status200OK)]
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
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CommentLikeDto>), StatusCodes.Status200OK)]
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
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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