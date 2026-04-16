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

    /// <summary>
    /// Gets notifications for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _mediator.Send(new GetUserNotificationsQuery(GetCurrentUserId(), unreadOnly, limit), cancellationToken);
        return Ok(notifications);
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    [HttpPost("{id:long}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarkNotificationAsReadCommand(id, GetCurrentUserId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        await _mediator.Send(new MarkAllNotificationsAsReadCommand(GetCurrentUserId()), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a notification.
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteNotificationCommand(id, GetCurrentUserId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}