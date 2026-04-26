using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.EpisodeComment.CreateEpisodeComment;
using ViewStream.Application.Commands.EpisodeComment.DeleteEpisodeComment;
using ViewStream.Application.Commands.EpisodeComment.UpdateEpisodeComment;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.EpisodeComment;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/episodes/{episodeId:long}/comments")]
[Produces("application/json")]
public class EpisodeCommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EpisodeCommentsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    private bool IsAdmin() =>
        User.IsInRole("SuperAdmin") || User.IsInRole("Moderator");

    #region Queries

    /// <summary>
    /// Retrieves root comments for an episode with pagination.
    /// </summary>
    /// <param name="episodeId">The ID of the episode.</param>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of root comments.</returns>
    /// <response code="200">Returns the paginated list of comments.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<EpisodeCommentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EpisodeCommentListItemDto>>> GetRootComments(
        long episodeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetRootCommentsPagedQuery(episodeId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves replies for a specific comment.
    /// </summary>
    /// <param name="commentId">The ID of the parent comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of reply comments.</returns>
    /// <response code="200">Returns the list of replies.</response>
    [HttpGet("replies/{commentId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<EpisodeCommentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EpisodeCommentListItemDto>>> GetReplies(
        long commentId,
        CancellationToken cancellationToken)
    {
        var replies = await _mediator.Send(new GetRepliesByParentQuery(commentId), cancellationToken);
        return Ok(replies);
    }

    /// <summary>
    /// Retrieves a single comment with nested replies up to a specified depth.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="maxDepth">Maximum depth of nested replies to include (default 3).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The comment with its reply tree.</returns>
    /// <response code="200">Returns the comment with nested replies.</response>
    /// <response code="404">Comment not found.</response>
    [HttpGet("{commentId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EpisodeCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeCommentDto>> GetComment(
        long commentId,
        [FromQuery] int maxDepth = 3,
        CancellationToken cancellationToken = default)
    {
        var comment = await _mediator.Send(new GetCommentWithRepliesQuery(commentId, maxDepth), cancellationToken);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new comment or reply.
    /// </summary>
    /// <param name="episodeId">The ID of the episode the comment belongs to.</param>
    /// <param name="dto">The comment data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created comment.</returns>
    /// <response code="201">Comment created successfully.</response>
    /// <response code="400">Episode ID mismatch or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EpisodeCommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateComment(
        long episodeId,
        [FromBody] CreateEpisodeCommentDto dto,
        CancellationToken cancellationToken)
    {
        if (episodeId != dto.EpisodeId)
            return BadRequest("Episode ID mismatch.");

        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var comment = await _mediator.Send(new CreateEpisodeCommentCommand(profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetComment), new { episodeId, commentId = comment.Id }, comment);
    }

    /// <summary>
    /// Updates an existing comment (owner only).
    /// </summary>
    /// <param name="commentId">The ID of the comment to update.</param>
    /// <param name="dto">The updated comment text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated comment.</returns>
    /// <response code="200">Comment updated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the owner of the comment.</response>
    /// <response code="404">Comment not found or already deleted.</response>
    [HttpPut("{commentId:long}")]
    [Authorize]
    [ProducesResponseType(typeof(EpisodeCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment(
        long commentId,
        [FromBody] UpdateEpisodeCommentDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var comment = await _mediator.Send(new UpdateEpisodeCommentCommand(commentId, profileId, dto, userId), cancellationToken);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    /// <summary>
    /// Soft deletes a comment (owner or admin/moderator).
    /// </summary>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Comment deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to delete this comment.</response>
    /// <response code="404">Comment not found or already deleted.</response>
    [HttpDelete("{commentId:long}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(
        long commentId,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var isAdmin = IsAdmin();
        var result = await _mediator.Send(new DeleteEpisodeCommentCommand(commentId, profileId, isAdmin, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}