using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Show.CreateShow;
using ViewStream.Application.Commands.Show.DeleteShow;
using ViewStream.Application.Commands.Show.RestoreShow;
using ViewStream.Application.Commands.Show.UpdateShow;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Show;

namespace ViewStream.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ShowsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShowsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets a paginated list of shows.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<ShowListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ShowListItemDto>>> GetShows(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] long? genreId = null,
            [FromQuery] int? year = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetShowsPagedQuery(page, pageSize, search, genreId, year), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a show by ID with full details.
        /// </summary>
        [HttpGet("{id:long}")]
        [AllowAnonymous]
        //[ProducesResponseType(typeof(ShowDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShowDto>> GetShow(long id, CancellationToken cancellationToken)
        {
            var show = await _mediator.Send(new GetShowByIdQuery(id), cancellationToken);
            if (show == null) return NotFound();
            return Ok(show);
        }

        /// <summary>
        /// Creates a new show (ContentManager/SuperAdmin only).
        /// </summary>
        [HttpPost]
        //[Authorize(Roles = "ContentManager,SuperAdmin")]
        //[ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShow([FromBody] CreateShowDto dto, CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var showId = await _mediator.Send(new CreateShowCommand(dto, userId), cancellationToken);
            return CreatedAtAction(nameof(GetShow), new { id = showId }, showId);
        }

        /// <summary>
        /// Updates an existing show.
        /// </summary>
        [HttpPut("{id:long}")]
        //[Authorize(Roles = "ContentManager,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShow(long id, [FromBody] UpdateShowDto dto, CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new UpdateShowCommand(id, dto, userId), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Soft deletes a show.
        /// </summary>
        [HttpDelete("{id:long}")]
        //[Authorize(Roles = "ContentManager,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShow(long id, CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new DeleteShowCommand(id, userId), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Restores a soft-deleted show.
        /// </summary>
        [HttpPost("{id:long}/restore")]
        //[Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RestoreShow(long id, CancellationToken cancellationToken)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new RestoreShowCommand(id, userId), cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
