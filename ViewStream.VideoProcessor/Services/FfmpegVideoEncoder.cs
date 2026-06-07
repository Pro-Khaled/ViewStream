using FFMpegCore;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.VideoProcessor.Services;

public class FfmpegVideoEncoder : IVideoEncoder
{
    public async Task<string> GenerateHlsAsync(string inputFilePath, string outputDirectory, List<HlsProfile> profiles, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDirectory);

        var scaleFilters = new List<string>();
        for (int i = 0; i < profiles.Count; i++)
        {
            scaleFilters.Add($"[0:v]scale={profiles[i].Resolution}:force_original_aspect_ratio=decrease[v{i}]");
        }
        var filterComplex = string.Join("; ", scaleFilters);

        var outputArgs = new List<string>();
        for (int i = 0; i < profiles.Count; i++)
        {
            outputArgs.Add($"-map \"[v{i}]\" -c:v:{i} libx264 -b:v:{i} {profiles[i].Bitrate} -maxrate:{i} {profiles[i].Maxrate} -bufsize:{i} {profiles[i].Bufsize}");
        }
        outputArgs.Add("-map a:0 -c:a aac -b:a 128k -ac 2");
        outputArgs.Add("-f hls -hls_time 10 -hls_list_size 0 -hls_playlist_type vod");
        outputArgs.Add($"-master_pl_name \"master.m3u8\"");
        outputArgs.Add($"-var_stream_map \"{string.Join(" ", profiles.Select((_, i) => $"v:{i},a:0"))}\"");
        outputArgs.Add($"{outputDirectory}/stream_%v.m3u8");

        var fullArgs = $"-i \"{inputFilePath}\" -filter_complex \"{filterComplex}\" {string.Join(" ", outputArgs)}";

        await FFMpegArguments
            .FromFileInput(inputFilePath)
            .OutputToFile(Path.Combine(outputDirectory, "master.m3u8"), true, options =>
            {
                options.WithCustomArgument(fullArgs);
            })
            .ProcessAsynchronously();

        return Path.Combine(outputDirectory, "master.m3u8");
    }

    public async Task<string> ExtractThumbnailAsync(string inputFilePath, string outputThumbnailPath, CancellationToken cancellationToken)
    {
        // Use string "png" instead of VideoCodec.Png
        await FFMpegArguments
            .FromFileInput(inputFilePath)
            .OutputToFile(outputThumbnailPath, true, options =>
            {
                options.WithVideoCodec("png")        // ✅ string codec
                       .WithFrameOutputCount(1)
                       .Seek(TimeSpan.FromSeconds(5));
            })
            .ProcessAsynchronously();

        return outputThumbnailPath;
    }

    public async Task<int> GetDurationAsync(string filePath, CancellationToken cancellationToken)
    {
        var mediaInfo = await FFProbe.AnalyseAsync(filePath, null, cancellationToken);
        return (int)mediaInfo.Duration.TotalSeconds;
    }
}