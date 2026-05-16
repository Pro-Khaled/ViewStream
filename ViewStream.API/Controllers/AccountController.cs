using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ViewStream.Application.Commands.Account.ConfirmEmail;
using ViewStream.Application.Commands.Account.ForgotPassword;
using ViewStream.Application.Commands.Account.Login;
using ViewStream.Application.Commands.Account.Logout;
using ViewStream.Application.Commands.Account.RefreshToken;
using ViewStream.Application.Commands.Account.Register;
using ViewStream.Application.Commands.Account.ResetPassword;
using ViewStream.Application.DTOs.Account;
using ViewStream.Application.Queries.User;

namespace ViewStream.Api.Controllers;

[ApiController]
[EnableRateLimiting("AuthRateLimit")]
[Route("api/v1/[controller]")]
[Produces("application/json")]

public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator) => _mediator = mediator;

    #region Registration & Verification

    /// <summary>
    /// Registers a new user account. Sends a confirmation email if required.
    /// </summary>
    /// <param name="model">Registration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message.</returns>
    /// <response code="200">Registration succeeded (or confirmation email resent).</response>
    /// <response code="400">Validation failed or email already taken.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]  

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

        var message = result.ConfirmationResent
            ? "This email is already registered but not confirmed. A new confirmation link has been sent."
            : "Registration successful. Please check your email to confirm your account.";

        return Ok(new
        {
            Message = message,
            RequiresEmailConfirmation = result.RequiresEmailConfirmation,
            ConfirmationResent = result.ConfirmationResent
        });
    }

    /// <summary>
    /// Confirms a user's email address using the token sent via email.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="token">Email confirmation token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message.</returns>
    /// <response code="200">Email confirmed successfully.</response>
    /// <response code="400">Invalid or expired token.</response>
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

    #endregion

    #region Password Reset

    /// <summary>
    /// Sends a password reset email if the given email address belongs to an active account.
    /// Always returns 200 to avoid email enumeration.
    /// </summary>
    /// <param name="model">The email address to send the reset link to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Always 200 OK.</returns>
    /// <response code="200">Request processed (email sent if account exists).</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _mediator.Send(new ForgotPasswordCommand(model.Email), cancellationToken);
        return Ok(new { Message = "If an account with that email exists, a password reset link has been sent." });
    }

    /// <summary>
    /// Resets a user's password using the token from the reset email.
    /// Revokes all existing refresh tokens on success.
    /// </summary>
    /// <param name="model">UserId, token, new password and confirmation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>200 on success, 400 on failure.</returns>
    /// <response code="200">Password reset successfully.</response>
    /// <response code="400">Invalid token, mismatched passwords or policy violation.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _mediator.Send(new ResetPasswordCommand(model), cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return BadRequest(ModelState);
        }

        return Ok(new { Message = "Password has been reset successfully. Please log in with your new password." });
    }

    #endregion

    #region Authentication

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens.
    /// </summary>
    /// <param name="model">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Access token, refresh token, and user profile.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Invalid credentials or account disabled.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]   
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
    /// <param name="model">The current refresh token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>New token pair.</returns>
    /// <response code="200">Tokens refreshed successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">Invalid or expired refresh token.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
    /// <param name="model">The refresh token to revoke.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Logout successful.</response>
    /// <response code="400">Invalid token.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var revoked = await _mediator.Send(new LogoutCommand(model.RefreshToken, userId), cancellationToken);
        if (!revoked)
            return BadRequest(new { Code = "INVALID_REFRESH_TOKEN", Message = "Refresh token not found or already revoked." });

        return NoContent();
    }

    #endregion

    #region Profile

    /// <summary>
    /// Retrieves the profile of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User profile.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">User not found or disabled.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            return Unauthorized(new { Code = "INVALID_TOKEN", Message = "Invalid access token." });

        var user = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        if (user == null)
            return NotFound(new { Code = "USER_NOT_FOUND", Message = "User not found or account disabled." });

        return Ok(user);
    }

    #endregion
}