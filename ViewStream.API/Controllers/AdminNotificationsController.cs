using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Notification.CreateNotification;
using ViewStream.Application.DTOs;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/notifications")]
[Authorize(Roles = "SuperAdmin,Support")]
[Produces("application/json")]
public class AdminNotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminNotificationsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    /// <param name="dto">The notification details including target user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created notification.</returns>
    /// <response code="201">Notification sent successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NotificationDto>> SendNotification(
        [FromBody] CreateNotificationDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var notification = await _mediator.Send(new CreateNotificationCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(SendNotification), null, notification);
    }
}