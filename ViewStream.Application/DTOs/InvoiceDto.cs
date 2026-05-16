using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of an invoice generated for a subscription payment.
    /// </summary>
    public class InvoiceDto
    {
        /// <summary>Gets or sets the unique identifier of the invoice.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the optional ID of the user billed.</summary>
        public long? UserId { get; set; }

        /// <summary>Gets or sets the email of the user billed.</summary>
        public string? UserEmail { get; set; }

        /// <summary>Gets or sets the optional ID of the subscription this invoice belongs to.</summary>
        public long? SubscriptionId { get; set; }

        /// <summary>Gets or sets the plan type of the subscription.</summary>
        public string? SubscriptionPlan { get; set; }

        /// <summary>Gets or sets the total amount billed.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the currency code (e.g. "USD").</summary>
        public string? Currency { get; set; }

        /// <summary>Gets or sets the current payment status (e.g. "paid", "pending", "failed").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the date the invoice was generated.</summary>
        public DateOnly InvoiceDate { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the invoice was successfully paid.</summary>
        public DateTime? PaidAt { get; set; }

        /// <summary>Gets or sets the URL to download a PDF copy of the invoice.</summary>
        public string? InvoicePdfUrl { get; set; }

        /// <summary>Gets or sets the external transaction ID from the payment gateway.</summary>
        public string? TransactionId { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user viewing their billing history.
    /// </summary>
    public class InvoiceListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the invoice.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the total amount billed.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the currency code.</summary>
        public string? Currency { get; set; }

        /// <summary>Gets or sets the current payment status.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the date the invoice was generated.</summary>
        public DateOnly InvoiceDate { get; set; }

        /// <summary>Gets or sets the URL to download the invoice PDF.</summary>
        public string? InvoicePdfUrl { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for invoices shown in the admin dashboard.
    /// </summary>
    public class AdminInvoiceListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the invoice.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the total amount billed.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the currency code.</summary>
        public string? Currency { get; set; }

        /// <summary>Gets or sets the current payment status.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the date the invoice was generated.</summary>
        public DateOnly InvoiceDate { get; set; }

        /// <summary>Gets or sets the URL to download the invoice PDF.</summary>
        public string? InvoicePdfUrl { get; set; }
    }
}
