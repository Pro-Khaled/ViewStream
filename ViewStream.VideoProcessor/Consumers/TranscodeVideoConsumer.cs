using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Contracts;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.VideoProcessor.Consumers
{
    public class TranscodeVideoConsumer : IConsumer<TranscodeVideoMessage>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IVideoEncoder _encoder;
        private readonly ILogger<TranscodeVideoConsumer> _logger;
        private readonly IOptions<List<HlsProfile>> _hlsProfiles;
        private readonly IEpisodeHubClient _episodeHubClient; // SignalR client

        public TranscodeVideoConsumer(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IVideoEncoder encoder,
            ILogger<TranscodeVideoConsumer> logger,
            IOptions<List<HlsProfile>> hlsProfiles,
            IEpisodeHubClient episodeHubClient)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _encoder = encoder;
            _logger = logger;
            _hlsProfiles = hlsProfiles;
            _episodeHubClient = episodeHubClient;
        }

        public async Task Consume(ConsumeContext<TranscodeVideoMessage> context)
        {
            var jobId = context.Message.JobId;
            _logger.LogInformation("Processing transcode job {JobId}", jobId);

            var job = await _unitOfWork.VideoProcessingJobs.GetByIdAsync(jobId);
            if (job == null)
            {
                _logger.LogError("Job {JobId} not found", jobId);
                return;
            }

            string localInputPath = null!;
            string outputDir = null!;
            string thumbLocalPath = null!;

            try
            {
                // Update status to Processing
                job.Status = "Processing";
                job.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                // 1. Download raw video to temp file
                localInputPath = Path.GetTempFileName() + Path.GetExtension(job.SourceFileUrl);
                await _fileStorage.DownloadToLocalAsync(job.SourceFileUrl, localInputPath);

                // 2. Generate HLS output using profiles
                outputDir = Path.Combine(Path.GetTempPath(), $"hls_{job.EpisodeId}_{Guid.NewGuid()}");
                var masterPlaylistPath = await _encoder.GenerateHlsAsync(localInputPath, outputDir, _hlsProfiles.Value, context.CancellationToken);

                // 3. Upload HLS folder to cloud/local storage
                var hlsRemoteFolder = $"episodes/{job.EpisodeId}/hls/";
                await _fileStorage.UploadDirectoryAsync(outputDir, hlsRemoteFolder);
                var masterRemoteUrl = _fileStorage.GetPublicUrl(hlsRemoteFolder + "master.m3u8");

                // 4. Extract thumbnail
                thumbLocalPath = Path.GetTempFileName() + ".png";
                await _encoder.ExtractThumbnailAsync(localInputPath, thumbLocalPath, context.CancellationToken);
                var thumbRemoteUrl = await _fileStorage.UploadFileAsync(thumbLocalPath, $"thumbnails/{job.EpisodeId}.png");

                // 5. Get video duration
                var duration = await _encoder.GetDurationAsync(localInputPath, context.CancellationToken);

                // 6. Update episode entity
                var episode = await _unitOfWork.Episodes.GetByIdAsync(job.EpisodeId);
                if (episode != null)
                {
                    episode.HlsMasterUrl = masterRemoteUrl;
                    episode.ThumbnailUrl = thumbRemoteUrl;
                    episode.DurationSeconds = duration;
                    episode.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Episodes.Update(episode);
                }

                // 7. Mark job as completed
                job.Status = "Completed";
                job.HlsMasterUrl = masterRemoteUrl;
                job.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                // 8. Notify clients via SignalR
                await _episodeHubClient.NotifyEpisodeUpdated(job.EpisodeId);

                _logger.LogInformation("Job {JobId} completed successfully", jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transcoding failed for job {JobId}", jobId);
                job.Status = "Failed";
                job.ErrorMessage = ex.ToString();
                job.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            }
            finally
            {
                // Cleanup temp files
                if (localInputPath != null && File.Exists(localInputPath)) File.Delete(localInputPath);
                if (outputDir != null && Directory.Exists(outputDir)) Directory.Delete(outputDir, true);
                if (thumbLocalPath != null && File.Exists(thumbLocalPath)) File.Delete(thumbLocalPath);
            }
        }
    }
}
