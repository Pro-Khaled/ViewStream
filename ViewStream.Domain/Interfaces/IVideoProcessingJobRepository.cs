using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    public interface IVideoProcessingJobRepository : IGenericRepository<VideoProcessingJob>
    {
        Task<VideoProcessingJob?> GetPendingByEpisodeIdAsync(long episodeId);
    }
}
