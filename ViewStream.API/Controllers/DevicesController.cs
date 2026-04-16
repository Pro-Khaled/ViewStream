using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Device.CreateDevice;
using ViewStream.Application.Commands.Device.DeleteDevice;
using ViewStream.Application.Commands.Device.UpdateDevice;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Device;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/users/me/devices")]
[Authorize]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DevicesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Gets all registered devices for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DeviceListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DeviceListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var devices = await _mediator.Send(new GetUserDevicesQuery(GetCurrentUserId()), cancellationToken);
        return Ok(devices);
    }

    /// <summary>
    /// Gets a specific device by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeviceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var device = await _mediator.Send(new GetDeviceByIdQuery(id, GetCurrentUserId()), cancellationToken);
        if (device == null) return NotFound();
        return Ok(device);
    }

    /// <summary>
    /// Registers a new device for the current user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DeviceDto>> Register([FromBody] CreateDeviceDto dto, CancellationToken cancellationToken)
    {
        var device = await _mediator.Send(new RegisterDeviceCommand(GetCurrentUserId(), dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
    }

    /// <summary>
    /// Updates a device (e.g., rename or trust).
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeviceDto>> Update(long id, [FromBody] UpdateDeviceDto dto, CancellationToken cancellationToken)
    {
        var device = await _mediator.Send(new UpdateDeviceCommand(id, GetCurrentUserId(), dto), cancellationToken);
        if (device == null) return NotFound();
        return Ok(device);
    }

    /// <summary>
    /// Removes a device from the user's account.
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDeviceCommand(id, GetCurrentUserId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}