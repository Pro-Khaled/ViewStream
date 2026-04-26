using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Profile.CreateProfile;
using ViewStream.Application.Commands.Profile.DeleteProfile;
using ViewStream.Application.Commands.Profile.SwitchActiveProfile;
using ViewStream.Application.Commands.Profile.UpdateProfile;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Profile;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all profiles for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of profiles belonging to the authenticated user.</returns>
    /// <response code="200">Returns the list of profiles.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProfileListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ProfileListItemDto>>> GetMyProfiles(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profiles = await _mediator.Send(new GetProfilesByUserQuery(userId), cancellationToken);
        return Ok(profiles);
    }

    /// <summary>
    /// Retrieves a specific profile by ID.
    /// </summary>
    /// <param name="id">The ID of the profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested profile.</returns>
    /// <response code="200">Returns the profile.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Profile does not belong to the current user.</response>
    /// <response code="404">Profile not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileDto>> GetProfile(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _mediator.Send(new GetProfileByIdQuery(id, userId), cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new profile for the current user.
    /// </summary>
    /// <param name="dto">The profile data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created profile.</returns>
    /// <response code="201">Profile created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProfile(
        [FromBody] CreateProfileDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _mediator.Send(new CreateProfileCommand(userId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
    }

    /// <summary>
    /// Updates an existing profile.
    /// </summary>
    /// <param name="id">The ID of the profile to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated profile.</returns>
    /// <response code="200">Profile updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Profile does not belong to the current user.</response>
    /// <response code="404">Profile not found or already deleted.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        long id,
        [FromBody] UpdateProfileDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _mediator.Send(new UpdateProfileCommand(id, userId, dto, userId), cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    /// <summary>
    /// Soft deletes a profile.
    /// </summary>
    /// <param name="id">The ID of the profile to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Profile deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Profile does not belong to the current user.</response>
    /// <response code="404">Profile not found or already deleted.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfile(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteProfileCommand(id, userId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Switches the active profile and returns a new JWT token with updated ProfileId claim.
    /// </summary>
    /// <param name="id">The ID of the profile to switch to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new JWT token and profile information.</returns>
    /// <response code="200">Profile switched successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Profile does not belong to the current user.</response>
    /// <response code="404">Profile not found or deleted.</response>
    [HttpPost("{id:long}/switch")]
    [ProducesResponseType(typeof(SwitchProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SwitchProfileResponseDto>> SwitchProfile(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var response = await _mediator.Send(new SwitchActiveProfileCommand(userId, id), cancellationToken);
        if (response == null) return NotFound();
        return Ok(response);
    }

    #endregion
}