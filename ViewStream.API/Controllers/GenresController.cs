using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<GenreListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GenreListItemDto>>> GetAllGenres(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllGenresQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GenreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> GetGenre(int id, CancellationToken cancellationToken)
    {
        var genre = await _mediator.Send(new GetGenreByIdQuery(id), cancellationToken);
        if (genre == null) return NotFound();
        return Ok(genre);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDto dto, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateGenreCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetGenre), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGenre(int id, [FromBody] UpdateGenreDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateGenreCommand(id, dto), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGenre(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteGenreCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}