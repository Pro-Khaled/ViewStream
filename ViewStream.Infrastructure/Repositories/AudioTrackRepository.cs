using ViewStream.Domain.Interfaces;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for AudioTrack entity
    /// </summary>
    public class AudioTrackRepository : GenericRepository<AudioTrack>, IAudioTrackRepository
    {
        public AudioTrackRepository(ViewStreamDbContext context) : base(context)
        {
        }

        // TODO: Implement custom methods specific to AudioTrack here
        // Example:
        // public async Task<> GetByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        // }
        //
        // public async Task<IEnumerable<AudioTrack>> GetActiveAsync()
        // {
        //     return await _dbSet.Where(x => x.IsActive).ToListAsync();
        // }
    }
}
