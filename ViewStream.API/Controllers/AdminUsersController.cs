using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.User.AdminUpdateUser;
using ViewStream.Application.Commands.User.BlockUser;
using ViewStream.Application.Commands.User.DeleteUser;
using ViewStream.Application.Commands.User.UnblockUser;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.User;

namespace ViewStream.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    //[Authorize(Roles = "SuperAdmin,UserManager")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminUsersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUsersPagedQuery(page, pageSize, search), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<UserDto>> GetUser(long id, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateUser(long id, AdminUpdateUserDto dto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AdminUpdateUserCommand(id, dto), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:long}/block")]
        public async Task<IActionResult> BlockUser(long id, BlockUserDto dto, CancellationToken cancellationToken)
        {
            var adminId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new BlockUserCommand(id, dto, adminId), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:long}/unblock")]
        public async Task<IActionResult> UnblockUser(long id, CancellationToken cancellationToken)
        {
            var adminId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new UnblockUserCommand(id, adminId), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken cancellationToken)
        {
            var adminId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new DeleteUserCommand(id, adminId), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}

