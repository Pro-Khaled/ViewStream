using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    public class VideoProcessingJobRepository : GenericRepository<VideoProcessingJob>, IVideoProcessingJobRepository
    {
        public VideoProcessingJobRepository(ViewStreamDbContext context) : base(context)
        {
        }

        public async Task<VideoProcessingJob?> GetPendingByEpisodeIdAsync(long episodeId)
        {
            return await _dbSet.FirstOrDefaultAsync(j => j.EpisodeId == episodeId && j.Status == "Pending");
        }
    }
}
