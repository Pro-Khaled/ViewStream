using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class InvoiceDto
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? UserEmail { get; set; }
        public long? SubscriptionId { get; set; }
        public string? SubscriptionPlan { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? InvoicePdfUrl { get; set; }
        public string? TransactionId { get; set; }
    }

    public class InvoiceListItemDto
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public string? InvoicePdfUrl { get; set; }
    }

}
