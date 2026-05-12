using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.DataDeletionRequest.CreateDataDeletionRequest;
using ViewStream.Application.Commands.DataDeletionRequest.DeleteDataDeletionRequest;
using ViewStream.Application.Commands.User.ChangePassword;
using ViewStream.Application.Commands.User.UpdateProfile;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.User;
using Microsoft.AspNetCore.RateLimiting;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("DefaultRateLimit")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves the profile of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or account disabled.</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var user = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        if (user == null) return NotFound();
        return Ok(user);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Updates the current user's profile (full name, phone, country).
    /// </summary>
    /// <param name="dto">The fields to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Profile updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPut("me")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateProfileCommand(userId, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    /// <param name="dto">Current and new password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Password changed successfully.</response>
    /// <response code="400">Invalid input or password policy violation.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found or deleted.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("change-password")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new ChangePasswordCommand(userId, dto, userId), cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return BadRequest(ModelState);
        }
        return NoContent();
    }

    #endregion

    #region Data Deletion Requests

    /// <summary>
    /// Submits a data deletion request for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created data deletion request.</returns>
    /// <response code="200">Request created or returned if already pending.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("me/data-deletion-request")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(typeof(DataDeletionRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<DataDeletionRequestDto>> RequestDataDeletion(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new CreateDataDeletionRequestCommand(userId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancels a pending data deletion request.
    /// </summary>
    /// <param name="id">The ID of the data deletion request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Request cancelled successfully.</response>
    /// <response code="400">Request cannot be cancelled (not pending or wrong state).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpDelete("me/data-deletion-request/{id:long}")]
    [EnableRateLimiting("UserWriteRateLimit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CancelDataDeletion(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteDataDeletionRequestCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion

    #region User Search

    /// <summary>
    /// Searches active users by name or email. Safe public endpoint for friend discovery.
    /// </summary>
    /// <param name="q">Search query (min 2 chars).</param>
    /// <param name="limit">Maximum results to return (default 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching users with public fields only.</returns>
    /// <response code="200">Returns matching users.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<UserPublicSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserPublicSearchResultDto>>> SearchUsers(
        [FromQuery] string q,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new List<UserPublicSearchResultDto>());

        var results = await _mediator.Send(new SearchUsersQuery(q, limit), cancellationToken);
        return Ok(results);
    }

    #endregion
}
