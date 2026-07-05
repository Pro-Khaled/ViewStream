using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Generates professional PDF invoices using QuestPDF.
    /// QuestPDF Community License is free for companies with less than $1M annual revenue.
    /// </summary>
    public class QuestPdfInvoiceService : IInvoicePdfService
    {
        private readonly ILogger<QuestPdfInvoiceService> _logger;

        public QuestPdfInvoiceService(ILogger<QuestPdfInvoiceService> logger)
        {
            _logger = logger;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Task<byte[]> GenerateAsync(long invoiceId, string userName, string userEmail,
            string planType, decimal amount, string currency, DateOnly invoiceDate, string? transactionId)
        {
            _logger.LogInformation("Generating PDF invoice #{InvoiceId} for {UserEmail}", invoiceId, userEmail);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(2, Unit.Centimetre);
                    page.MarginVertical(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // ── Header ──────────────────────────────────────────────
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("ViewStream")
                                    .FontSize(28).Bold().FontColor(Colors.Blue.Darken2);
                                left.Item().Text("Streaming Platform")
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                            });

                            row.ConstantItem(150).AlignRight().Column(right =>
                            {
                                right.Item().Text("INVOICE")
                                    .FontSize(24).Bold().FontColor(Colors.Grey.Darken2);
                                right.Item().Text($"#{invoiceId:D6}")
                                    .FontSize(12).FontColor(Colors.Grey.Darken1);
                            });
                        });

                        col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
                    });

                    // ── Content ─────────────────────────────────────────────
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // Invoice & Customer details in two columns
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("Bill To:").Bold().FontSize(12);
                                left.Item().PaddingTop(4).Text(userName).FontSize(11);
                                left.Item().Text(userEmail)
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantItem(200).AlignRight().Column(right =>
                            {
                                right.Item().Text("Invoice Details").Bold().FontSize(12);
                                right.Item().PaddingTop(4).Text($"Date: {invoiceDate:yyyy-MM-dd}")
                                    .FontSize(10);
                                right.Item().Text($"Status: Paid")
                                    .FontSize(10).FontColor(Colors.Green.Darken2);
                                if (!string.IsNullOrEmpty(transactionId))
                                {
                                    right.Item().Text($"Transaction: {transactionId}")
                                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                                }
                            });
                        });

                        col.Item().PaddingVertical(20);

                        // Line items table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);  // Description
                                columns.RelativeColumn(1);  // Qty
                                columns.RelativeColumn(2);  // Unit Price
                                columns.RelativeColumn(2);  // Total
                            });

                            // Table header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Darken2)
                                    .Padding(6).Text("Description")
                                    .FontColor(Colors.White).Bold().FontSize(10);
                                header.Cell().Background(Colors.Blue.Darken2)
                                    .Padding(6).AlignCenter().Text("Qty")
                                    .FontColor(Colors.White).Bold().FontSize(10);
                                header.Cell().Background(Colors.Blue.Darken2)
                                    .Padding(6).AlignRight().Text("Unit Price")
                                    .FontColor(Colors.White).Bold().FontSize(10);
                                header.Cell().Background(Colors.Blue.Darken2)
                                    .Padding(6).AlignRight().Text("Total")
                                    .FontColor(Colors.White).Bold().FontSize(10);
                            });

                            // Subscription line item
                            var bgColor = Colors.Grey.Lighten4;
                            table.Cell().Background(bgColor)
                                .Padding(6).Text($"ViewStream {planType} Plan – Monthly Subscription")
                                .FontSize(10);
                            table.Cell().Background(bgColor)
                                .Padding(6).AlignCenter().Text("1").FontSize(10);
                            table.Cell().Background(bgColor)
                                .Padding(6).AlignRight().Text($"{amount:F2} {currency}").FontSize(10);
                            table.Cell().Background(bgColor)
                                .Padding(6).AlignRight().Text($"{amount:F2} {currency}").FontSize(10);
                        });

                        col.Item().PaddingVertical(10);

                        // Total summary
                        col.Item().AlignRight().Width(250).Column(totalCol =>
                        {
                            totalCol.Item().Row(row =>
                            {
                                row.RelativeItem().AlignRight().Padding(4)
                                    .Text("Subtotal:").FontSize(10);
                                row.ConstantItem(100).AlignRight().Padding(4)
                                    .Text($"{amount:F2} {currency}").FontSize(10);
                            });
                            totalCol.Item().Row(row =>
                            {
                                row.RelativeItem().AlignRight().Padding(4)
                                    .Text("Tax:").FontSize(10);
                                row.ConstantItem(100).AlignRight().Padding(4)
                                    .Text($"0.00 {currency}").FontSize(10);
                            });
                            totalCol.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            totalCol.Item().Row(row =>
                            {
                                row.RelativeItem().AlignRight().Padding(4)
                                    .Text("Total:").Bold().FontSize(12);
                                row.ConstantItem(100).AlignRight().Padding(4)
                                    .Text($"{amount:F2} {currency}").Bold().FontSize(12)
                                    .FontColor(Colors.Blue.Darken2);
                            });
                        });
                    });

                    // ── Footer ──────────────────────────────────────────────
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(8).AlignCenter()
                            .Text("Thank you for subscribing to ViewStream!")
                            .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(2).AlignCenter()
                            .Text("support@viewstream.com | www.viewstream.com")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            _logger.LogInformation("PDF invoice generated, {Size} bytes", pdfBytes.Length);
            return Task.FromResult(pdfBytes);
        }
    }
}
