using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for ErrorLog entity
    /// </summary>
    public interface IErrorLogRepository : IGenericRepository<ErrorLog>
    {
        // TODO: Add custom methods specific to ErrorLog here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<ErrorLog>> GetActiveAsync();
    }
}
