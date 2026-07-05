namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Buffers progress updates in-memory and flushes to DB periodically to reduce write pressure.
    /// </summary>
    public interface IProgressBufferService
    {
        /// <summary>Buffer a progress update. Will be flushed to DB within 5 seconds.</summary>
        Task BufferProgressAsync(long profileId, long episodeId, int progressSeconds);

        /// <summary>Manually flush all buffered progress updates to DB.</summary>
        Task FlushAsync();
    }
}
