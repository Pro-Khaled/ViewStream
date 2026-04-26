using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.Genre.CreateGenre;
using ViewStream.Application.Commands.Genre.DeleteGenre;
using ViewStream.Application.Commands.Genre.UpdateGenre;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Genre;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GenresController : ControllerBase
{
    private readonly IMediator _mediator;

    public GenresController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of genres.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="search">Optional search term to filter by genre name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of genres.</returns>
    /// <response code="200">Returns the paginated list of genres.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<GenreListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<GenreListItemDto>>> GetGenres(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetGenresPagedQuery(page, pageSize, search), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all genres (useful for dropdowns).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all genres.</returns>
    /// <response code="200">Returns the list of all genres.</response>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<GenreListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GenreListItemDto>>> GetAllGenres(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllGenresQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single genre by ID.
    /// </summary>
    /// <param name="id">The ID of the genre.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested genre.</returns>
    /// <response code="200">Returns the genre.</response>
    /// <response code="404">Genre not found.</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> GetGenre(
        int id,
        CancellationToken cancellationToken)
    {
        var genre = await _mediator.Send(new GetGenreByIdQuery(id), cancellationToken);
        if (genre == null) return NotFound();
        return Ok(genre);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new genre.
    /// </summary>
    /// <param name="dto">The data for the new genre.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created genre.</returns>
    /// <response code="201">Genre created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGenre(
        [FromBody] CreateGenreDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var id = await _mediator.Send(new CreateGenreCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetGenre), new { id }, id);
    }

    /// <summary>
    /// Updates an existing genre.
    /// </summary>
    /// <param name="id">The ID of the genre to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Genre updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Genre not found.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGenre(
        int id,
        [FromBody] UpdateGenreDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateGenreCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a genre.
    /// </summary>
    /// <param name="id">The ID of the genre to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Genre deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Genre not found.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGenre(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteGenreCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}