using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload;
using ViewStream.Application.Commands.OfflineDownload.DeleteOfflineDownload;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.OfflineDownload;

namespace ViewStream.Api.Controllers;

[ApiController]
[Route("api/profiles/me/downloads")]
[Authorize]
[Produces("application/json")]
public class OfflineDownloadsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OfflineDownloadsController(IMediator mediator) => _mediator = mediator;

    private long GetCurrentProfileId() =>
        long.Parse(User.FindFirstValue("ProfileId") ?? "0");

    /// <summary>
    /// Gets all offline downloads for the current profile.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OfflineDownloadListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OfflineDownloadListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var downloads = await _mediator.Send(new GetProfileDownloadsQuery(GetCurrentProfileId()), cancellationToken);
        return Ok(downloads);
    }

    /// <summary>
    /// Creates a new offline download record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OfflineDownloadDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<OfflineDownloadDto>> Create([FromBody] CreateOfflineDownloadDto dto, CancellationToken cancellationToken)
    {
        var download = await _mediator.Send(new CreateOfflineDownloadCommand(GetCurrentProfileId(), dto), cancellationToken);
        return CreatedAtAction(nameof(GetAll), null, download);
    }

    /// <summary>
    /// Deletes an offline download record (does not delete the actual file).
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteOfflineDownloadCommand(id, GetCurrentProfileId()), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}