using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Queries.Analytics;

namespace ViewStream.API.Controllers.Admin
{
    /// <summary>
    /// Admin-only controller for exporting analytics reports as CSV files.
    /// </summary>
    [ApiController]
    [Route("api/admin/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Exports cohort retention data as a CSV file.
        /// Each row represents a cohort month with retention percentages for subsequent months.
        /// </summary>
        /// <param name="cohortMonths">Number of cohort months to include (default: 6).</param>
        [HttpGet("export/retention")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExportRetentionCsv([FromQuery] int cohortMonths = 6)
        {
            _logger.LogInformation("Exporting cohort retention CSV with {CohortMonths} months", cohortMonths);

            var data = await _mediator.Send(new GetCohortRetentionQuery(cohortMonths));

            // Flatten the nested retention data into flat CSV rows
            var csvRows = new List<RetentionCsvRow>();
            foreach (var cohort in data)
            {
                foreach (var point in cohort.RetentionData)
                {
                    csvRows.Add(new RetentionCsvRow
                    {
                        CohortMonth = cohort.CohortMonth,
                        TotalUsers = cohort.TotalUsers,
                        MonthOffset = point.MonthOffset,
                        ActiveUsers = point.ActiveUsers,
                        RetentionRate = point.RetentionRate
                    });
                }
            }

            var csvBytes = WriteCsv(csvRows);

            var fileName = $"retention_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            _logger.LogInformation("Retention CSV generated: {FileName}, {RowCount} rows", fileName, csvRows.Count);

            return File(csvBytes, "text/csv", fileName);
        }

        /// <summary>
        /// Exports trending shows data as a CSV file.
        /// Shows are ranked by trend score based on recent playback events with time decay.
        /// </summary>
        /// <param name="days">Number of days to look back for trending data (default: 7).</param>
        /// <param name="limit">Maximum number of shows to include (default: 50).</param>
        /// <param name="countryCode">Optional country code filter.</param>
        [HttpGet("export/trending")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExportTrendingCsv(
            [FromQuery] int days = 7,
            [FromQuery] int limit = 50,
            [FromQuery] string? countryCode = null)
        {
            _logger.LogInformation("Exporting trending shows CSV: Days={Days}, Limit={Limit}, Country={Country}",
                days, limit, countryCode);

            var data = await _mediator.Send(new GetTrendingShowsQuery(days, limit, countryCode));

            var csvRows = data.Select((show, index) => new TrendingCsvRow
            {
                Rank = index + 1,
                ShowId = show.ShowId,
                Title = show.Title,
                PosterUrl = show.PosterUrl,
                ViewCount = show.ViewCount,
                TrendScore = show.TrendScore
            }).ToList();

            var csvBytes = WriteCsv(csvRows);

            var fileName = $"trending_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            _logger.LogInformation("Trending CSV generated: {FileName}, {RowCount} rows", fileName, csvRows.Count);

            return File(csvBytes, "text/csv", fileName);
        }

        /// <summary>
        /// Writes a list of objects to a CSV byte array using CsvHelper.
        /// </summary>
        private static byte[] WriteCsv<T>(IEnumerable<T> records)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, leaveOpen: true);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            csv.WriteRecords(records);
            writer.Flush();

            return memoryStream.ToArray();
        }

        // ── CSV row models ──────────────────────────────────────────

        private class RetentionCsvRow
        {
            public string CohortMonth { get; set; } = string.Empty;
            public int TotalUsers { get; set; }
            public int MonthOffset { get; set; }
            public int ActiveUsers { get; set; }
            public decimal RetentionRate { get; set; }
        }

        private class TrendingCsvRow
        {
            public int Rank { get; set; }
            public long ShowId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? PosterUrl { get; set; }
            public int ViewCount { get; set; }
            public decimal TrendScore { get; set; }
        }
    }
}
