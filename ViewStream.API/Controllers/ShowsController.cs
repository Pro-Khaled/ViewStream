using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Show.CreateShow;
using ViewStream.Application.Commands.Show.DeleteShow;
using ViewStream.Application.Commands.Show.RestoreShow;
using ViewStream.Application.Commands.Show.UpdateShow;
using ViewStream.Application.Commands.Show.UploadShowfiles;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Show;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ShowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShowsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of shows with optional filters.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="search">Optional search term for title.</param>
    /// <param name="genreId">Optional filter by genre ID.</param>
    /// <param name="year">Optional filter by release year.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of shows.</returns>
    /// <response code="200">Returns the paginated list of shows.</response>
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
    /// Retrieves a single show by ID with full details.
    /// </summary>
    /// <param name="id">The ID of the show.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested show.</returns>
    /// <response code="200">Returns the show.</response>
    /// <response code="404">Show not found.</response>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShowDto>> GetShow(long id, CancellationToken cancellationToken)
    {
        var show = await _mediator.Send(new GetShowByIdQuery(id), cancellationToken);
        if (show == null) return NotFound();
        return Ok(show);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new show with optional genre and tag assignments.
    /// </summary>
    /// <param name="dto">The show data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created show.</returns>
    /// <response code="201">Show created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateShow(
        [FromBody] CreateShowDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var showId = await _mediator.Send(new CreateShowCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetShow), new { id = showId }, showId);
    }

    /// <summary>
    /// Updates an existing show, including genres and tags.
    /// </summary>
    /// <param name="id">The ID of the show to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Show updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Show not found or already deleted.</response>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShow(
        long id,
        [FromBody] UpdateShowDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateShowCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a show and cascades to its seasons and episodes.
    /// </summary>
    /// <param name="id">The ID of the show to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Show deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Show not found or already deleted.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShow(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteShowCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Restores a soft-deleted show.
    /// </summary>
    /// <param name="id">The ID of the show to restore.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Show restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not a SuperAdmin.</response>
    /// <response code="404">Show not found or not deleted.</response>
    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreShow(
        long id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new RestoreShowCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Uploads a poster image for a show.
    /// </summary>
    /// <param name="id">The ID of the show.</param>
    /// <param name="posterFile">The poster image file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded poster.</returns>
    /// <response code="200">Poster uploaded successfully.</response>
    /// <response code="400">No file provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Show not found.</response>
    [HttpPost("{id:long}/upload-poster")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadPoster(
        long id,
        IFormFile posterFile,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var posterUrl = await _mediator.Send(new UploadShowPosterCommand(id, posterFile, userId), cancellationToken);
        return Ok(new { posterUrl });
    }

    /// <summary>
    /// Uploads a backdrop image for a show.
    /// </summary>
    /// <param name="id">The ID of the show.</param>
    /// <param name="backdropFile">The backdrop image file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded backdrop.</returns>
    /// <response code="200">Backdrop uploaded successfully.</response>
    /// <response code="400">No file provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Show not found.</response>
    [HttpPost("{id:long}/upload-backdrop")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadBackdrop(
        long id,
        IFormFile backdropFile,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var backdropUrl = await _mediator.Send(new UploadShowBackdropCommand(id, backdropFile, userId), cancellationToken);
        return Ok(new { backdropUrl });
    }

    /// <summary>
    /// Uploads a trailer video for a show.
    /// </summary>
    /// <param name="id">The ID of the show.</param>
    /// <param name="trailerFile">The trailer video file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded trailer.</returns>
    /// <response code="200">Trailer uploaded successfully.</response>
    /// <response code="400">No file provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Show not found.</response>
    [HttpPost("{id:long}/upload-trailer")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadTrailer(
        long id,
        IFormFile trailerFile,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var trailerUrl = await _mediator.Send(new UploadShowTrailerCommand(id, trailerFile, userId), cancellationToken);
        return Ok(new { trailerUrl });
    }

    #endregion
}