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

    private bool IsAdmin() =>
        User.IsInRole("SuperAdmin") || User.IsInRole("Moderator");

    /// <summary>
    /// Gets root comments for an episode (paginated).
    /// </summary>
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
    /// Gets replies for a specific comment.
    /// </summary>
    [HttpGet("replies/{commentId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<EpisodeCommentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EpisodeCommentListItemDto>>> GetReplies(long commentId, CancellationToken cancellationToken)
    {
        var replies = await _mediator.Send(new GetRepliesByParentQuery(commentId), cancellationToken);
        return Ok(replies);
    }

    /// <summary>
    /// Gets a single comment with nested replies (limited depth).
    /// </summary>
    [HttpGet("{commentId:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EpisodeCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpisodeCommentDto>> GetComment(long commentId, [FromQuery] int maxDepth = 3, CancellationToken cancellationToken = default)
    {
        var comment = await _mediator.Send(new GetCommentWithRepliesQuery(commentId, maxDepth), cancellationToken);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    /// <summary>
    /// Creates a new comment or reply.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EpisodeCommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateComment(
        long episodeId,
        [FromBody] CreateEpisodeCommentDto dto,
        CancellationToken cancellationToken)
    {
        if (episodeId != dto.EpisodeId)
            return BadRequest("Episode ID mismatch.");

        var profileId = GetCurrentProfileId();
        var comment = await _mediator.Send(new CreateEpisodeCommentCommand(profileId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetComment), new { episodeId, commentId = comment.Id }, comment);
    }

    /// <summary>
    /// Updates an existing comment (owner only).
    /// </summary>
    [HttpPut("{commentId:long}")]
    [Authorize]
    [ProducesResponseType(typeof(EpisodeCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateComment(
        long commentId,
        [FromBody] UpdateEpisodeCommentDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var comment = await _mediator.Send(new UpdateEpisodeCommentCommand(commentId, profileId, dto), cancellationToken);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    /// <summary>
    /// Soft deletes a comment (owner or admin/mod).
    /// </summary>
    [HttpDelete("{commentId:long}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteComment(long commentId, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var isAdmin = IsAdmin();
        var result = await _mediator.Send(new DeleteEpisodeCommentCommand(commentId, profileId, isAdmin), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}