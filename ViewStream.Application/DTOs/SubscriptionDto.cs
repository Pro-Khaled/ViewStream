using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class SubscriptionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string PlanType { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? AutoRenew { get; set; }
        public long? PaymentMethodId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public string PlanType { get; set; } = string.Empty;
        public long? PaymentMethodId { get; set; }
        public bool? AutoRenew { get; set; } = true;
    }
    public class UpdateSubscriptionDto
    {
        public string? PlanType { get; set; }
        public string? Status { get; set; }
        public bool? AutoRenew { get; set; }
        public long? PaymentMethodId { get; set; }
    }
}
