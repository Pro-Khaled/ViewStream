using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class DeviceDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string? DeviceName { get; set; }
        public string? Platform { get; set; }
        public DateTime? LastActive { get; set; }
        public bool? IsTrusted { get; set; }
    }

    public class DeviceListItemDto
    {
        public long Id { get; set; }
        public string? DeviceName { get; set; }
        public string? Platform { get; set; }
        public DateTime? LastActive { get; set; }
        public bool? IsTrusted { get; set; }
    }

    public class CreateDeviceDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string? DeviceName { get; set; }
        public string? Platform { get; set; }
    }

    public class UpdateDeviceDto
    {
        public string? DeviceName { get; set; }
        public bool? IsTrusted { get; set; }
    }


}
