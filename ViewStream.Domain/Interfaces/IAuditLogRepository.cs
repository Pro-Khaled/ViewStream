using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for AuditLog entity
    /// </summary>
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        // TODO: Add custom methods specific to AuditLog here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<AuditLog>> GetActiveAsync();
    }
}
