using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Notification.DeleteNotification;
using ViewStream.Application.Commands.Notification.MarkAllNotificationsAsRead;
using ViewStream.Application.Commands.Notification.MarkNotificationAsRead;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Notification;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/users/me/notifications")]
[Authorize]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves notifications for the current user.
    /// </summary>
    /// <param name="unreadOnly">If true, returns only unread notifications.</param>
    /// <param name="limit">Maximum number of notifications to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of notifications.</returns>
    /// <response code="200">Returns the list of notifications.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _mediator.Send(new GetUserNotificationsQuery(GetCurrentUserId(), unreadOnly, limit), cancellationToken);
        return Ok(notifications);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="id">The ID of the notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Notification marked as read successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Notification does not belong to the current user.</response>
    /// <response code="404">Notification not found.</response>
    [HttpPost("{id:long}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new MarkNotificationAsReadCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">All notifications marked as read successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _mediator.Send(new MarkAllNotificationsAsReadCommand(userId, userId), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a notification.
    /// </summary>
    /// <param name="id">The ID of the notification to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Notification deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Notification does not belong to the current user.</response>
    /// <response code="404">Notification not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteNotificationCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}