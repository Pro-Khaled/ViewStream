using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.AuditLog;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/admin/audit/logs")]
[Authorize(Roles = "SuperAdmin,Auditor")]
[Produces("application/json")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminAuditLogsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Gets paginated audit logs with extensive filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogListItemDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tableName = null,
        [FromQuery] long? recordId = null,
        [FromQuery] string? action = null,
        [FromQuery] long? changedByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuditLogsPagedQuery(page, pageSize, tableName, recordId, action, changedByUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific audit log by ID (includes full old/new values).
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditLogDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _mediator.Send(new GetAuditLogByIdQuery(id), cancellationToken);
        if (log == null) return NotFound();
        return Ok(log);
    }
}