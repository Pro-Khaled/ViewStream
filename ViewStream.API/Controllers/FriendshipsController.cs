using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Friendship.BlockUser;
using ViewStream.Application.Commands.Friendship.RespondToFriendRequest;
using ViewStream.Application.Commands.Friendship.SendFriendRequest;
using ViewStream.Application.Commands.Friendship.Unfriend;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Friendship;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/friends")]
[Authorize]
[Produces("application/json")]
public class FriendshipsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FriendshipsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a friendship summary for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Summary counts of friends, pending requests, and blocked users.</returns>
    /// <response code="200">Returns the friendship summary.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(FriendshipSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FriendshipSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new GetFriendshipSummaryQuery(userId), cancellationToken));
    }

    /// <summary>
    /// Retrieves all accepted friends of the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of accepted friends.</returns>
    /// <response code="200">Returns the list of friends.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FriendshipListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<FriendshipListItemDto>>> GetFriends(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new GetFriendsQuery(userId, "accepted"), cancellationToken));
    }

    /// <summary>
    /// Retrieves pending friend requests (sent or received).
    /// </summary>
    /// <param name="direction">"sent" or "received" (default "received").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of pending friend requests.</returns>
    /// <response code="200">Returns the list of pending requests.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<FriendshipListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<FriendshipListItemDto>>> GetPendingRequests(
        [FromQuery] string direction = "received",
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new GetPendingRequestsQuery(userId, direction), cancellationToken));
    }

    /// <summary>
    /// Searches among the current user's friends.
    /// </summary>
    /// <param name="query">Search term (matches name or email).</param>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of matching friends.</returns>
    /// <response code="200">Returns the search results.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<FriendshipListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<FriendshipListItemDto>>> SearchFriends(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new SearchFriendsQuery(userId, query, page, pageSize), cancellationToken));
    }

    #endregion

    #region Commands

    /// <summary>
    /// Sends a friend request to another user.
    /// </summary>
    /// <param name="dto">The target friend ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created friendship record.</returns>
    /// <response code="200">Friend request sent successfully.</response>
    /// <response code="400">Invalid request (e.g., self-request, already friends, blocked).</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("request")]
    [ProducesResponseType(typeof(FriendshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FriendshipDto>> SendRequest(
        [FromBody] FriendRequestDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new SendFriendRequestCommand(userId, dto, userId), cancellationToken));
    }

    /// <summary>
    /// Accepts or declines a pending friend request.
    /// </summary>
    /// <param name="friendId">The ID of the user who sent the request.</param>
    /// <param name="dto">The new status ("accepted" or "blocked").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated friendship record.</returns>
    /// <response code="200">Request processed successfully.</response>
    /// <response code="400">Invalid status.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Pending request not found.</response>
    [HttpPut("request/{friendId:long}")]
    [ProducesResponseType(typeof(FriendshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FriendshipDto>> RespondToRequest(
        long friendId,
        [FromBody] UpdateFriendshipStatusDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RespondToFriendRequestCommand(userId, friendId, dto, userId), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Blocks a user.
    /// </summary>
    /// <param name="friendId">The ID of the user to block.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated friendship record.</returns>
    /// <response code="200">User blocked successfully.</response>
    /// <response code="400">Cannot block yourself.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("block/{friendId:long}")]
    [ProducesResponseType(typeof(FriendshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FriendshipDto>> BlockUser(
        long friendId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new BlockUserCommand(userId, friendId, userId), cancellationToken));
    }

    /// <summary>
    /// Unfriends a user (removes the friendship relationship).
    /// </summary>
    /// <param name="friendId">The ID of the friend to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Friend removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Friendship not found.</response>
    [HttpDelete("{friendId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unfriend(
        long friendId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UnfriendCommand(userId, friendId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}