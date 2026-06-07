using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services
{
    public interface IVideoEncoder
    {
        Task<string> GenerateHlsAsync(string inputFilePath, string outputDirectory, List<HlsProfile> profiles, CancellationToken cancellationToken);
        Task<string> ExtractThumbnailAsync(string inputFilePath, string outputThumbnailPath, CancellationToken cancellationToken);
        Task<int> GetDurationAsync(string filePath, CancellationToken cancellationToken);
    }

    public class HlsProfile
    {
        public string Resolution { get; set; } = string.Empty;
        public string Bitrate { get; set; } = string.Empty;
        public string Maxrate { get; set; } = string.Empty;
        public string Bufsize { get; set; } = string.Empty;
    }
}
