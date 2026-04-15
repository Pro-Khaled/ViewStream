using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class PaymentMethodDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string? LastFour { get; set; }
        public string? CardType { get; set; }
        public short? ExpiryMonth { get; set; }
        public short? ExpiryYear { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreatePaymentMethodDto
    {
        public string Provider { get; set; } = string.Empty;
        public string ProviderToken { get; set; } = string.Empty;
        public string? LastFour { get; set; }
        public string? CardType { get; set; }
        public short? ExpiryMonth { get; set; }
        public short? ExpiryYear { get; set; }
        public bool? IsDefault { get; set; }
    }

    public class UpdatePaymentMethodDto
    {
        public short? ExpiryMonth { get; set; }
        public short? ExpiryYear { get; set; }
        public bool? IsDefault { get; set; }
    }

}
