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

    #region Queries

    /// <summary>
    /// Retrieves all registered devices for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of devices belonging to the authenticated user.</returns>
    /// <response code="200">Returns the list of devices.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<DeviceListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DeviceListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var devices = await _mediator.Send(new GetUserDevicesQuery(GetCurrentUserId()), cancellationToken);
        return Ok(devices);
    }

    /// <summary>
    /// Retrieves a specific device by ID.
    /// </summary>
    /// <param name="id">The ID of the device.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested device.</returns>
    /// <response code="200">Returns the device.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not own this device.</response>
    /// <response code="404">Device not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeviceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var device = await _mediator.Send(new GetDeviceByIdQuery(id, GetCurrentUserId()), cancellationToken);
        if (device == null) return NotFound();
        return Ok(device);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Registers a new device for the current user.
    /// </summary>
    /// <param name="dto">The device information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly registered device.</returns>
    /// <response code="201">Device registered successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DeviceDto>> Register([FromBody] CreateDeviceDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var device = await _mediator.Send(new RegisterDeviceCommand(userId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
    }

    /// <summary>
    /// Updates a device (e.g., rename or trust).
    /// </summary>
    /// <param name="id">The ID of the device to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated device.</returns>
    /// <response code="200">Device updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not own this device.</response>
    /// <response code="404">Device not found.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeviceDto>> Update(long id, [FromBody] UpdateDeviceDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var device = await _mediator.Send(new UpdateDeviceCommand(id, userId, dto, userId), cancellationToken);
        if (device == null) return NotFound();
        return Ok(device);
    }

    /// <summary>
    /// Removes a device from the user's account.
    /// </summary>
    /// <param name="id">The ID of the device to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Device removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not own this device.</response>
    /// <response code="404">Device not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteDeviceCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}