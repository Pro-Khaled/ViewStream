using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    /// Sends a notification to a specific user (admin only).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<NotificationDto>> SendNotification([FromBody] CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        var notification = await _mediator.Send(new CreateNotificationCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(SendNotification), null, notification);
    }
}