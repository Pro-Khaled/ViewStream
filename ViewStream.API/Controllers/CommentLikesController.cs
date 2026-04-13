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

    /// <summary>
    /// Gets reaction summary for a comment.
    /// </summary>
    [HttpGet("summary")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CommentReactionSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CommentReactionSummaryDto>> GetReactionSummary(long commentId, CancellationToken cancellationToken)
    {
        var profileId = User.Identity?.IsAuthenticated == true ? GetCurrentProfileId() : (long?)null;
        var summary = await _mediator.Send(new GetCommentReactionSummaryQuery(commentId, profileId), cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Gets all reactions for a comment.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CommentLikeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CommentLikeDto>>> GetReactions(long commentId, CancellationToken cancellationToken)
    {
        var reactions = await _mediator.Send(new GetReactionsByCommentQuery(commentId), cancellationToken);
        return Ok(reactions);
    }

    /// <summary>
    /// Gets the current user's reaction for a comment.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentLikeDto>> GetMyReaction(long commentId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var reaction = await _mediator.Send(new GetUserReactionForCommentQuery(commentId, profileId), cancellationToken);
        if (reaction == null) return NotFound();
        return Ok(reaction);
    }

    /// <summary>
    /// Adds or updates a reaction on a comment.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CommentLikeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentLikeDto>> React(long commentId, [FromBody] CreateUpdateCommentLikeDto dto, CancellationToken cancellationToken)
    {
        if (commentId != dto.CommentId)
            return BadRequest("Comment ID mismatch.");

        var profileId = GetCurrentProfileId();
        var reaction = await _mediator.Send(new UpsertCommentLikeCommand(profileId, dto), cancellationToken);
        return Ok(reaction);
    }

    /// <summary>
    /// Removes the current user's reaction from a comment.
    /// </summary>
    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveReaction(long commentId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var result = await _mediator.Send(new DeleteCommentLikeCommand(commentId, profileId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}