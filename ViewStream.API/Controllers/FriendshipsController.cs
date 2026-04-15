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

    /// <summary>Gets friendship summary for the current user.</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<FriendshipSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new GetFriendshipSummaryQuery(userId), cancellationToken));
    }

    /// <summary>Gets all accepted friends.</summary>
    [HttpGet]
    public async Task<ActionResult<List<FriendshipListItemDto>>> GetFriends(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new GetFriendsQuery(userId, "accepted"), cancellationToken));
    }

    /// <summary>Gets pending friend requests (sent or received).</summary>
    [HttpGet("pending")]
    public async Task<ActionResult<List<FriendshipListItemDto>>> GetPendingRequests([FromQuery] string direction = "received", CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new GetPendingRequestsQuery(userId, direction), cancellationToken));
    }

    /// <summary>Searches among friends.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<FriendshipListItemDto>>> SearchFriends([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new SearchFriendsQuery(userId, query, page, pageSize), cancellationToken));
    }

    /// <summary>Sends a friend request.</summary>
    [HttpPost("request")]
    public async Task<ActionResult<FriendshipDto>> SendRequest([FromBody] FriendRequestDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new SendFriendRequestCommand(userId, dto), cancellationToken));
    }

    /// <summary>Accepts or declines a friend request.</summary>
    [HttpPut("request/{friendId:long}")]
    public async Task<ActionResult<FriendshipDto>> RespondToRequest(long friendId, [FromBody] UpdateFriendshipStatusDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RespondToFriendRequestCommand(userId, friendId, dto), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Blocks a user.</summary>
    [HttpPost("block/{friendId:long}")]
    public async Task<ActionResult<FriendshipDto>> BlockUser(long friendId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await _mediator.Send(new BlockUserCommand(userId, friendId), cancellationToken));
    }

    /// <summary>Unfriends a user (removes friendship).</summary>
    [HttpDelete("{friendId:long}")]
    public async Task<IActionResult> Unfriend(long friendId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UnfriendCommand(userId, friendId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}