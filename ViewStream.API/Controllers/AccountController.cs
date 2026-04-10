using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.DTOs.Account;
using ViewStream.Application.Features.Account.Commands.ConfirmEmail;
using ViewStream.Application.Features.Account.Commands.Login;
using ViewStream.Application.Features.Account.Commands.Logout;
using ViewStream.Application.Features.Account.Commands.RefreshToken;
using ViewStream.Application.Features.Account.Commands.Register;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Services;

namespace ViewStream.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(new RegisterCommand(model), cancellationToken);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);
                return BadRequest(ModelState);
            }

            if (result.RequiresEmailConfirmation)
            {
                var message = result.ConfirmationResent
                    ? "This email was already registered but not confirmed. A new confirmation link has been sent to your email."
                    : "Registration successful. Please check your email to confirm your account.";

                return Ok(new
                {
                    Message = message,
                    RequiresEmailConfirmation = true,
                    ConfirmationResent = result.ConfirmationResent
                });
            }

            return Ok(new { Message = "Registration successful." });
        }


        [HttpGet("confirm-email")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(
    [FromQuery] long userId,
    [FromQuery] string token,
    CancellationToken cancellationToken)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(token))
                return BadRequest(new { Message = "Invalid confirmation link." });

            var result = await _mediator.Send(new ConfirmEmailCommand(userId, token), cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { Message = result.ErrorMessage ?? "Email confirmation failed." });

            return Ok(new { Message = "Email confirmed successfully. You can now log in." });
        }

        ///// <summary>
        ///// Authenticates a user and issues a JWT access token and refresh token.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        //[ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(
            [FromBody] LoginDto model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _mediator.Send(new LoginCommand(model), cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Code = "AUTH_FAILED", Message = ex.Message });
            }
        }

        /// <summary>
        /// Exchanges a valid refresh token for a new access token and refresh token pair.
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        //[ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Refresh(
            [FromBody] RefreshTokenDto model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _mediator.Send(new RefreshTokenCommand(model.RefreshToken), cancellationToken);
            if (response == null)
                return Unauthorized(new { Code = "INVALID_REFRESH_TOKEN", Message = "Invalid or expired refresh token." });

            return Ok(response);
        }

        /// <summary>
        /// Logs out the user by revoking the provided refresh token.
        /// </summary>
        [HttpPost("logout")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(
            [FromBody] RefreshTokenDto model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var revoked = await _mediator.Send(new LogoutCommand(model.RefreshToken), cancellationToken);
            if (!revoked)
                return BadRequest(new { Code = "INVALID_REFRESH_TOKEN", Message = "Refresh token not found or already revoked." });

            return NoContent();
        }



        /// <summary>
        /// Retrieves the profile of the currently authenticated user.
        /// </summary>
        //    [HttpGet("me")]
        //    [Authorize]
        //    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        //    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //    [ProducesResponseType(StatusCodes.Status404NotFound)]
        //    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
        //    {
        //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
        //    return Unauthorized(new { Code = "INVALID_TOKEN", Message = "Invalid access token." });

        //var user = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        //if (user == null)
        //    return NotFound(new { Code = "USER_NOT_FOUND", Message = "User not found or account disabled." });

        //return Ok(user);
        //    }
    }
}

