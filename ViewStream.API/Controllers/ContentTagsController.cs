using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

    /// <summary>
    /// Gets a paginated list of content tags.
    /// </summary>
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
    /// Gets all content tags (for dropdowns).
    /// </summary>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ContentTagListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContentTagListItemDto>>> GetAllContentTags(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllContentTagsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets content tags by category.
    /// </summary>
    [HttpGet("category/{category}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ContentTagListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContentTagListItemDto>>> GetContentTagsByCategory(string category, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContentTagsByCategoryQuery(category), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a content tag by ID with full details.
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ContentTagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentTagDto>> GetContentTag(int id, CancellationToken cancellationToken)
    {
        var tag = await _mediator.Send(new GetContentTagByIdQuery(id), cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    /// <summary>
    /// Creates a new content tag (ContentManager/SuperAdmin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContentTag([FromBody] CreateContentTagDto dto, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new CreateContentTagCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetContentTag), new { id }, id);
    }

    /// <summary>
    /// Updates an existing content tag.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContentTag(int id, [FromBody] UpdateContentTagDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateContentTagCommand(id, dto), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a content tag.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ContentManager,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContentTag(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteContentTagCommand(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

}