using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        // Episode
        Task<string> SaveVideoAsync(IFormFile file, long episodeId, CancellationToken cancellationToken = default);
        Task<string> SaveThumbnailAsync(IFormFile file, long episodeId, CancellationToken cancellationToken = default);

        // Show
        Task<string> SavePosterAsync(IFormFile file, long showId, CancellationToken cancellationToken = default);
        Task<string> SaveBackdropAsync(IFormFile file, long showId, CancellationToken cancellationToken = default);
        Task<string> SaveTrailerAsync(IFormFile file, long showId, CancellationToken cancellationToken = default);

        // Subtitle
        Task<string> SaveSubtitleFileAsync(IFormFile file, long subtitleId, CancellationToken cancellationToken = default);

        // Audio Track
        Task<string> SaveAudioFileAsync(IFormFile file, long audioTrackId, CancellationToken cancellationToken = default);
        void DeleteFile(string filePath);
    }

}
