using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ViewStreamDbContext context) : base(context)
        {
        }
    }
}
