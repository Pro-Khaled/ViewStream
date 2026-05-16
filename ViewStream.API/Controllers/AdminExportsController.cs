using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Api.Controllers;

/// <summary>
/// Administrative controller that streams filtered data as CSV downloads.
/// All endpoints require <c>AdminRateLimit</c> and at minimum SuperAdmin role.
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[EnableRateLimiting("AdminRateLimit")]
[Produces("application/json")]
public class AdminExportsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises a new instance of <see cref="AdminExportsController"/>.</summary>
    public AdminExportsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    // ─── helpers ────────────────────────────────────────────────────────────────

    private static string EscapeCsv(string? value)
    {
        if (value is null) return string.Empty;
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static string Ts(DateTime? dt) => dt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
    private static string Ds(DateOnly? d) => d?.ToString("yyyy-MM-dd") ?? string.Empty;

    private IActionResult CsvStream(string filename, Func<StreamWriter, CancellationToken, Task> writer, CancellationToken ct)
    {
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        return new StreamWriterResult(Response.Body, writer, ct);
    }

    // ─── Audit Logs ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Exports filtered audit log records as a CSV file.
    /// Applies the same filters as the paged admin audit log query but without pagination.
    /// </summary>
    /// <param name="request">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file attachment.</returns>
    /// <response code="200">CSV file streamed successfully.</response>
    /// <response code="401">Unauthorized – authentication required.</response>
    /// <response code="403">Forbidden – insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("audit/logs/export")]
    [Authorize(Roles = "SuperAdmin,Auditor")]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    public async Task ExportAuditLogs([FromBody] ExportAuditLogsRequest request, CancellationToken cancellationToken)
    {
        var filename = $"export_audit_logs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        var query = _unitOfWork.AuditLogs.GetQueryable()
            .Include(e => e.ChangedByUser)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(x =>
                x.TableName.Contains(term) ||
                x.Action.Contains(term) ||
                (x.ChangedByUser != null && x.ChangedByUser.UserName.Contains(term)));
        }
        if (!string.IsNullOrWhiteSpace(request.TableName))
            query = query.Where(x => x.TableName == request.TableName);
        if (request.RecordId.HasValue)
            query = query.Where(x => x.RecordId == request.RecordId.Value);
        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(x => x.Action == request.Action);
        if (request.ChangedByUserId.HasValue)
            query = query.Where(x => x.ChangedByUserId == request.ChangedByUserId.Value);
        if (request.From.HasValue)
            query = query.Where(x => x.ChangedAt >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.ChangedAt <= request.To.Value);

        query = query.OrderByDescending(x => x.ChangedAt);

        await using var sw = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);
        await sw.WriteLineAsync("Id,TableName,RecordId,Action,OldValues,NewValues,ChangedByUserEmail,ChangedAt,IpAddress,UserAgent,Notes");

        await foreach (var row in query
            .Select(s => new
            {
                s.Id, s.TableName, s.RecordId, s.Action,
                s.OldValues, s.NewValues,
                ChangedByUserEmail = s.ChangedByUser != null ? s.ChangedByUser.Email : null,
                s.ChangedAt, s.IpAddress, s.UserAgent, s.Notes
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            var line = string.Join(",",
                row.Id,
                EscapeCsv(row.TableName),
                row.RecordId,
                EscapeCsv(row.Action),
                EscapeCsv(row.OldValues),
                EscapeCsv(row.NewValues),
                EscapeCsv(row.ChangedByUserEmail),
                Ts(row.ChangedAt),
                EscapeCsv(row.IpAddress),
                EscapeCsv(row.UserAgent),
                EscapeCsv(row.Notes));
            await sw.WriteLineAsync(line);
        }
    }

    // ─── Error Logs ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Exports filtered error log records as a CSV file.
    /// Applies the same filters as the paged admin error log query but without pagination.
    /// </summary>
    /// <param name="request">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file attachment.</returns>
    /// <response code="200">CSV file streamed successfully.</response>
    /// <response code="401">Unauthorized – authentication required.</response>
    /// <response code="403">Forbidden – insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("errors/logs/export")]
    [Authorize(Roles = "SuperAdmin,Support")]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    public async Task ExportErrorLogs([FromBody] ExportErrorLogsRequest request, CancellationToken cancellationToken)
    {
        var filename = $"export_error_logs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        var query = _unitOfWork.ErrorLogs.GetQueryable()
            .Include(e => e.User)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(x =>
                (x.ErrorMessage != null && x.ErrorMessage.Contains(term)) ||
                (x.Endpoint != null && x.Endpoint.Contains(term)) ||
                (x.ErrorCode != null && x.ErrorCode.Contains(term)));
        }
        if (!string.IsNullOrWhiteSpace(request.ErrorCode))
            query = query.Where(x => x.ErrorCode == request.ErrorCode);
        if (!string.IsNullOrWhiteSpace(request.Endpoint))
            query = query.Where(x => x.Endpoint == request.Endpoint);
        if (request.From.HasValue)
            query = query.Where(x => x.OccurredAt >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.OccurredAt <= request.To.Value);

        query = query.OrderByDescending(x => x.OccurredAt);

        await using var sw = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);
        await sw.WriteLineAsync("Id,UserEmail,ErrorCode,ErrorMessage,StackTrace,Endpoint,OccurredAt");

        await foreach (var row in query
            .Select(s => new
            {
                s.Id,
                UserEmail = s.User != null ? s.User.Email : null,
                s.ErrorCode, s.ErrorMessage, s.StackTrace, s.Endpoint, s.OccurredAt
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            var line = string.Join(",",
                row.Id,
                EscapeCsv(row.UserEmail),
                EscapeCsv(row.ErrorCode),
                EscapeCsv(row.ErrorMessage),
                EscapeCsv(row.StackTrace),
                EscapeCsv(row.Endpoint),
                Ts(row.OccurredAt));
            await sw.WriteLineAsync(line);
        }
    }

    // ─── Search Logs ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Exports filtered search log records as a CSV file.
    /// Applies the same filters as the paged admin search log query but without pagination.
    /// </summary>
    /// <param name="request">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file attachment.</returns>
    /// <response code="200">CSV file streamed successfully.</response>
    /// <response code="401">Unauthorized – authentication required.</response>
    /// <response code="403">Forbidden – insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("search/logs/export")]
    [Authorize(Roles = "SuperAdmin,Analytics")]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    public async Task ExportSearchLogs([FromBody] ExportSearchLogsRequest request, CancellationToken cancellationToken)
    {
        var filename = $"export_search_logs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        var query = _unitOfWork.SearchLogs.GetQueryable()
            .Include(e => e.Profile)
            .Include(e => e.ClickedShow)
            .AsNoTracking();

        if (request.ProfileId.HasValue)
            query = query.Where(x => x.ProfileId == request.ProfileId.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm) || !string.IsNullOrWhiteSpace(request.Query))
        {
            var term = (request.SearchTerm ?? request.Query ?? string.Empty).Trim();
            query = query.Where(x =>
                x.Query.Contains(term) ||
                (x.Profile != null && x.Profile.Name.Contains(term)) ||
                (x.ClickedShow != null && x.ClickedShow.Title.Contains(term)));
        }
        if (request.From.HasValue)
            query = query.Where(x => x.SearchAt >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.SearchAt <= request.To.Value);

        query = query.OrderByDescending(x => x.SearchAt);

        await using var sw = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);
        await sw.WriteLineAsync("Id,ProfileName,Query,ResultsCount,ClickedShowTitle,SearchAt");

        await foreach (var row in query
            .Select(s => new
            {
                s.Id,
                ProfileName = s.Profile != null ? s.Profile.Name : null,
                s.Query, s.ResultsCount,
                ClickedShowTitle = s.ClickedShow != null ? s.ClickedShow.Title : null,
                s.SearchAt
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            var line = string.Join(",",
                row.Id,
                EscapeCsv(row.ProfileName),
                EscapeCsv(row.Query),
                row.ResultsCount?.ToString() ?? string.Empty,
                EscapeCsv(row.ClickedShowTitle),
                Ts(row.SearchAt));
            await sw.WriteLineAsync(line);
        }
    }

    // ─── Invoices ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Exports filtered invoice records as a CSV file.
    /// Applies the same filters as the paged admin invoice query but without pagination.
    /// </summary>
    /// <param name="request">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file attachment.</returns>
    /// <response code="200">CSV file streamed successfully.</response>
    /// <response code="401">Unauthorized – authentication required.</response>
    /// <response code="403">Forbidden – insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("invoices/export")]
    [Authorize(Roles = "SuperAdmin,Finance")]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    public async Task ExportInvoices([FromBody] ExportInvoicesRequest request, CancellationToken cancellationToken)
    {
        var filename = $"export_invoices_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        var query = _unitOfWork.Invoices.GetQueryable()
            .Include(e => e.User)
            .Include(e => e.Subscription)
            .AsNoTracking();

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);
        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.Status == request.Status);
        if (request.From.HasValue)
            query = query.Where(x => x.InvoiceDate >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.InvoiceDate <= request.To.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(x =>
                (x.TransactionId != null && x.TransactionId.Contains(term)) ||
                (x.Status != null && x.Status.Contains(term)));
        }

        query = query.OrderByDescending(x => x.InvoiceDate);

        await using var sw = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);
        await sw.WriteLineAsync("Id,UserEmail,SubscriptionPlan,Amount,Currency,Status,InvoiceDate,PaidAt,TransactionId");

        await foreach (var row in query
            .Select(s => new
            {
                s.Id,
                UserEmail = s.User != null ? s.User.Email : null,
                SubscriptionPlan = s.Subscription != null ? s.Subscription.PlanType : null,
                s.Amount, s.Currency, s.Status, s.InvoiceDate, s.PaidAt, s.TransactionId
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            var line = string.Join(",",
                row.Id,
                EscapeCsv(row.UserEmail),
                EscapeCsv(row.SubscriptionPlan),
                row.Amount,
                EscapeCsv(row.Currency),
                EscapeCsv(row.Status),
                Ds(row.InvoiceDate),
                Ts(row.PaidAt),
                EscapeCsv(row.TransactionId));
            await sw.WriteLineAsync(line);
        }
    }

    // ─── User Interactions ───────────────────────────────────────────────────────

    /// <summary>
    /// Exports filtered user interaction records as a CSV file.
    /// Applies the same filters as the paged admin user interaction query but without pagination.
    /// </summary>
    /// <param name="request">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file attachment.</returns>
    /// <response code="200">CSV file streamed successfully.</response>
    /// <response code="401">Unauthorized – authentication required.</response>
    /// <response code="403">Forbidden – insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("userinteractions/export")]
    [Authorize(Roles = "SuperAdmin,Analytics")]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    public async Task ExportUserInteractions([FromBody] ExportUserInteractionsRequest request, CancellationToken cancellationToken)
    {
        var filename = $"export_userinteractions_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        var query = _unitOfWork.UserInteractions.GetQueryable()
            .Include(e => e.Profile)
            .Include(e => e.Show)
            .AsNoTracking();

        if (request.ProfileId.HasValue)
            query = query.Where(x => x.ProfileId == request.ProfileId.Value);
        if (request.ShowId.HasValue)
            query = query.Where(x => x.ShowId == request.ShowId.Value);
        if (!string.IsNullOrWhiteSpace(request.InteractionType))
            query = query.Where(x => x.InteractionType == request.InteractionType);
        if (request.FromDate.HasValue)
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(x =>
                x.InteractionType.Contains(term) ||
                (x.Show != null && x.Show.Title.Contains(term)) ||
                (x.Profile != null && x.Profile.Name.Contains(term)));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        await using var sw = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);
        await sw.WriteLineAsync("Id,ProfileName,ShowTitle,InteractionType,Weight,CreatedAt");

        await foreach (var row in query
            .Select(s => new
            {
                s.Id,
                ProfileName = s.Profile != null ? s.Profile.Name : null,
                ShowTitle = s.Show != null ? s.Show.Title : null,
                s.InteractionType, s.Weight, s.CreatedAt
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            var line = string.Join(",",
                row.Id,
                EscapeCsv(row.ProfileName),
                EscapeCsv(row.ShowTitle),
                EscapeCsv(row.InteractionType),
                row.Weight?.ToString() ?? string.Empty,
                Ts(row.CreatedAt));
            await sw.WriteLineAsync(line);
        }
    }

    // ─── Playback Events ─────────────────────────────────────────────────────────

    /// <summary>
    /// Exports filtered playback event records as a CSV file.
    /// Applies the same filters as the paged admin playback event query but without pagination.
    /// </summary>
    /// <param name="request">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <c>text/csv</c> file attachment.</returns>
    /// <response code="200">CSV file streamed successfully.</response>
    /// <response code="401">Unauthorized – authentication required.</response>
    /// <response code="403">Forbidden – insufficient permissions.</response>
    /// <response code="429">Too many requests. Please wait before trying again.</response>
    [HttpPost("playbackevents/export")]
    [Authorize(Roles = "SuperAdmin,Analytics")]
    [Produces("text/csv")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    public async Task ExportPlaybackEvents([FromBody] ExportPlaybackEventsRequest request, CancellationToken cancellationToken)
    {
        var filename = $"export_playbackevents_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";
        Response.ContentType = "text/csv; charset=utf-8";

        var query = _unitOfWork.PlaybackEvents.GetQueryable()
            .Include(e => e.Profile)
            .Include(e => e.Episode)
            .AsNoTracking();

        if (request.EpisodeId.HasValue)
            query = query.Where(x => x.EpisodeId == request.EpisodeId.Value);
        if (request.ProfileId.HasValue)
            query = query.Where(x => x.ProfileId == request.ProfileId.Value);
        if (request.From.HasValue)
            query = query.Where(x => x.CreatedAt >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.CreatedAt <= request.To.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(x =>
                x.EventType.Contains(term) ||
                (x.Episode != null && x.Episode.Title.Contains(term)) ||
                (x.Profile != null && x.Profile.Name.Contains(term)));
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        await using var sw = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);
        await sw.WriteLineAsync("Id,ProfileName,EpisodeTitle,EventType,PositionSeconds,Quality,BitrateKbps,DeviceType,CreatedAt");

        await foreach (var row in query
            .Select(s => new
            {
                s.Id,
                ProfileName = s.Profile != null ? s.Profile.Name : null,
                EpisodeTitle = s.Episode != null ? s.Episode.Title : null,
                s.EventType, s.PositionSeconds, s.Quality, s.BitrateKbps, s.DeviceType, s.CreatedAt
            })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            var line = string.Join(",",
                row.Id,
                EscapeCsv(row.ProfileName),
                EscapeCsv(row.EpisodeTitle),
                EscapeCsv(row.EventType),
                row.PositionSeconds?.ToString() ?? string.Empty,
                EscapeCsv(row.Quality),
                row.BitrateKbps?.ToString() ?? string.Empty,
                EscapeCsv(row.DeviceType),
                Ts(row.CreatedAt));
            await sw.WriteLineAsync(line);
        }
    }
}

/// <summary>
/// Internal helper action result that writes a CSV directly to the response body stream
/// without buffering the full dataset in memory.
/// </summary>
internal sealed class StreamWriterResult : IActionResult
{
    private readonly Stream _body;
    private readonly Func<StreamWriter, CancellationToken, Task> _writer;
    private readonly CancellationToken _ct;

    public StreamWriterResult(Stream body, Func<StreamWriter, CancellationToken, Task> writer, CancellationToken ct)
    {
        _body = body;
        _writer = writer;
        _ct = ct;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        await using var sw = new StreamWriter(_body, Encoding.UTF8, leaveOpen: true);
        await _writer(sw, _ct);
        await sw.FlushAsync();
    }
}
