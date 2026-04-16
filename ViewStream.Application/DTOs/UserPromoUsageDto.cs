using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class UserPromoUsageDto
    {
        public long UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int PromoCodeId { get; set; }
        public string PromoCodeCode { get; set; } = string.Empty;
        public DateTime? UsedAt { get; set; }
    }

}
