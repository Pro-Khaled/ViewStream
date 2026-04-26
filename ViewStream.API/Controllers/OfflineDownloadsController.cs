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

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    #region Queries

    /// <summary>
    /// Retrieves all offline downloads for the current profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of offline downloads with episode and device details.</returns>
    /// <response code="200">Returns the list of downloads.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OfflineDownloadListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<OfflineDownloadListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var downloads = await _mediator.Send(new GetProfileDownloadsQuery(GetCurrentProfileId()), cancellationToken);
        return Ok(downloads);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new offline download record.
    /// </summary>
    /// <param name="dto">The download details including episode and device.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created offline download record.</returns>
    /// <response code="201">Download record created successfully.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(OfflineDownloadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OfflineDownloadDto>> Create(
        [FromBody] CreateOfflineDownloadDto dto,
        CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var download = await _mediator.Send(new CreateOfflineDownloadCommand(profileId, dto, userId), cancellationToken);
        return CreatedAtAction(nameof(GetAll), null, download);
    }

    /// <summary>
    /// Deletes an offline download record (does not delete the actual file).
    /// </summary>
    /// <param name="id">The ID of the download record to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Download record deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Download does not belong to the current profile.</response>
    /// <response code="404">Download record not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var profileId = GetCurrentProfileId();
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteOfflineDownloadCommand(id, profileId, userId), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}