using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _webRootPath;                     // Physical root (wwwroot)
    private readonly string _videoFolder = "uploads/episodes";
    private readonly string _thumbnailFolder = "uploads/thumbnails";
    private readonly string _posterFolder = "uploads/shows/posters";
    private readonly string _backdropFolder = "uploads/shows/backdrops";
    private readonly string _trailerFolder = "uploads/shows/trailers";
    private readonly string _subtitleFolder = "uploads/subtitles";
    private readonly string _audioFolder = "uploads/audio";


    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    // Episode files

    public async Task<string> SaveVideoAsync(IFormFile file, long episodeId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".mp4", ".mkv", ".webm", ".mov" };
        return await SaveFileAsync(file, episodeId, _videoFolder, allowedExtensions, cancellationToken);
    }

    public async Task<string> SaveThumbnailAsync(IFormFile file, long episodeId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return await SaveFileAsync(file, episodeId, _thumbnailFolder, allowedExtensions, cancellationToken);
    }


    // Show files


    public async Task<string> SavePosterAsync(IFormFile file, long showId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return await SaveFileAsync(file, showId, _posterFolder, allowedExtensions, cancellationToken);
    }

    public async Task<string> SaveBackdropAsync(IFormFile file, long showId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return await SaveFileAsync(file, showId, _backdropFolder, allowedExtensions, cancellationToken);
    }

    public async Task<string> SaveTrailerAsync(IFormFile file, long showId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".mp4", ".webm", ".mov" };
        return await SaveFileAsync(file, showId, _trailerFolder, allowedExtensions, cancellationToken);
    }


    // Subtitle files

    public async Task<string> SaveSubtitleFileAsync(IFormFile file, long subtitleId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".vtt", ".srt", ".ass", ".ssa" };
        return await SaveFileAsync(file, subtitleId, _subtitleFolder, allowedExtensions, cancellationToken);
    }



    // Audio files

    public async Task<string> SaveAudioFileAsync(IFormFile file, long audioTrackId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".mp3", ".aac", ".opus", ".m4a", ".flac" };
        return await SaveFileAsync(file, audioTrackId, _audioFolder, allowedExtensions, cancellationToken);
    }

    private async Task<string> SaveFileAsync(IFormFile file, long episodeId, string folder, string[] allowedExtensions, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException($"Invalid file format. Allowed: {string.Join(", ", allowedExtensions)}");

        var fileName = $"ep_{episodeId}_{Guid.NewGuid()}{extension}";
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), folder);
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/{folder}/{fileName}";
    }


    public void DeleteFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;

        var fullPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public async Task DownloadToLocalAsync(string remotePath, string localPath)
    {
        var fullSourcePath = Path.Combine(_webRootPath, remotePath.TrimStart('/'));
        if (!File.Exists(fullSourcePath))
            throw new FileNotFoundException($"File not found: {fullSourcePath}");
        using var sourceStream = new FileStream(fullSourcePath, FileMode.Open, FileAccess.Read);
        using var destStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
        await sourceStream.CopyToAsync(destStream);
    }

    public async Task UploadDirectoryAsync(string localDirectory, string remotePrefix)
    {
        var targetDir = Path.Combine(_webRootPath, remotePrefix.TrimStart('/'));
        Directory.CreateDirectory(targetDir);
        foreach (var filePath in Directory.GetFiles(localDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(localDirectory, filePath);
            var destPath = Path.Combine(targetDir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
            using var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write);
            await sourceStream.CopyToAsync(destStream);
        }
    }

    public async Task<string> UploadFileAsync(string localPath, string remotePath)
    {
        var destPath = Path.Combine(_webRootPath, remotePath.TrimStart('/'));
        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
        using var sourceStream = new FileStream(localPath, FileMode.Open, FileAccess.Read);
        using var destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write);
        await sourceStream.CopyToAsync(destStream);
        return remotePath; // returns the relative path (e.g., "thumbnails/123.png")
    }

    public string GetPublicUrl(string remotePath)
    {
        // Return a relative URL that the static files middleware can serve.
        if (!remotePath.StartsWith('/'))
            remotePath = "/" + remotePath;
        return remotePath;
    }
}