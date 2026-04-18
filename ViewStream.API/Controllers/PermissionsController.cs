using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Commands.Permission.CreatePermission;
using ViewStream.Application.Commands.Permission.DeletePermission;
using ViewStream.Application.Commands.Permission.UpdatePermission;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Permission;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/permissions")]
[Authorize(Roles = "SuperAdmin")]
[Produces("application/json")]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PermissionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets all permissions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PermissionListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PermissionListItemDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetAllPermissionsQuery(), cancellationToken));

    /// <summary>
    /// Gets all permissions grouped by GroupName.
    /// </summary>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(Dictionary<string, List<PermissionListItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, List<PermissionListItemDto>>>> GetGrouped(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetPermissionsByGroupQuery(), cancellationToken));

    /// <summary>
    /// Gets a permission by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var perm = await _mediator.Send(new GetPermissionByIdQuery(id), cancellationToken);
        return perm == null ? NotFound() : Ok(perm);
    }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionDto dto, CancellationToken cancellationToken)
    {
        var perm = await _mediator.Send(new CreatePermissionCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = perm.Id }, perm);
    }

    /// <summary>
    /// Updates an existing permission.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> Update(int id, [FromBody] UpdatePermissionDto dto, CancellationToken cancellationToken)
    {
        var perm = await _mediator.Send(new UpdatePermissionCommand(id, dto), cancellationToken);
        return perm == null ? NotFound() : Ok(perm);
    }

    /// <summary>
    /// Deletes a permission.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePermissionCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}