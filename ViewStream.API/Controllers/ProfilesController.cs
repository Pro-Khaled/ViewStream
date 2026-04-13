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

    /// <summary>
    /// Gets all profiles for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProfileListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProfileListItemDto>>> GetMyProfiles(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profiles = await _mediator.Send(new GetProfilesByUserQuery(userId), cancellationToken);
        return Ok(profiles);
    }

    /// <summary>
    /// Gets a specific profile by ID (must belong to the current user).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileDto>> GetProfile(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _mediator.Send(new GetProfileByIdQuery(id, userId), cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    /// <summary>
    /// Creates a new profile for the current user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _mediator.Send(new CreateProfileCommand(userId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
    }

    /// <summary>
    /// Updates an existing profile.
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(long id, [FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _mediator.Send(new UpdateProfileCommand(id, userId, dto), cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    /// <summary>
    /// Soft deletes a profile.
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfile(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteProfileCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Switches the active profile and returns a new JWT token with the updated ProfileId claim.
    /// </summary>
    [HttpPost("{id:long}/switch")]
    [ProducesResponseType(typeof(SwitchProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SwitchProfileResponseDto>> SwitchProfile(long id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var response = await _mediator.Send(new SwitchActiveProfileCommand(userId, id), cancellationToken);
        if (response == null) return NotFound();
        return Ok(response);
    }
}