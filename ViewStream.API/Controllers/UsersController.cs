using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.User.ChangePassword;
using ViewStream.Application.Commands.User.UpdateProfile;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.User;

namespace ViewStream.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto, CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new UpdateProfileCommand(userId, dto), cancellationToken);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new ChangePasswordCommand(userId, dto), cancellationToken);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}

