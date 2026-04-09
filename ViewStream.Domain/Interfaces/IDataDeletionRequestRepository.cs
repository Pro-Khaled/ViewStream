using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for DataDeletionRequest entity
    /// </summary>
    public interface IDataDeletionRequestRepository : IGenericRepository<DataDeletionRequest>
    {
        // TODO: Add custom methods specific to DataDeletionRequest here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<DataDeletionRequest>> GetActiveAsync();
    }
}
