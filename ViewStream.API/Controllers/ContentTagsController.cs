using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.ContentTag.CreateContentTag;
using ViewStream.Application.Commands.ContentTag.DeleteContentTag;
using ViewStream.Application.Commands.ContentTag.UpdateContentTag;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.ContentTag;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ContentTagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentTagsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves a paginated list of content tags.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="search">Optional search term to filter by tag name or category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of content tags.</returns>
    /// <response code="200">Returns the paginated list of content tags.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<ContentTagListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ContentTagListItemDto>>> GetContentTags(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetContentTagsPagedQuery(page, pageSize, search), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all content tags (useful for dropdowns).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all content tags.</returns>
    /// <response code="200">Returns the list of all content tags.</response>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ContentTagListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContentTagListItemDto>>> GetAllContentTags(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllContentTagsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves content tags filtered by category.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of content tags belonging to the specified category.</returns>
    /// <response code="200">Returns the filtered list of content tags.</response>
    [HttpGet("category/{category}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ContentTagListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContentTagListItemDto>>> GetContentTagsByCategory(
        string category,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContentTagsByCategoryQuery(category), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single content tag by ID with full details.
    /// </summary>
    /// <param name="id">The ID of the content tag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested content tag.</returns>
    /// <response code="200">Returns the content tag.</response>
    /// <response code="404">Content tag not found.</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ContentTagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentTagDto>> GetContentTag(
        int id,
        CancellationToken cancellationToken)
    {
        var tag = await _mediator.Send(new GetContentTagByIdQuery(id), cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new content tag.
    /// </summary>
    /// <param name="dto">The data for the new content tag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created content tag.</returns>
    /// <response code="201">Content tag created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateContentTag(
        [FromBody] CreateContentTagDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var id = await _mediator.Send(new CreateContentTagCommand(dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetContentTag), new { id }, id);
    }

    /// <summary>
    /// Updates an existing content tag.
    /// </summary>
    /// <param name="id">The ID of the content tag to update.</param>
    /// <param name="dto">The updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Content tag updated successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Content tag not found.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContentTag(
        int id,
        [FromBody] UpdateContentTagDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new UpdateContentTagCommand(id, dto, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a content tag.
    /// </summary>
    /// <param name="id">The ID of the content tag to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Content tag deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission.</response>
    /// <response code="404">Content tag not found.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContentTag(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteContentTagCommand(id, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}