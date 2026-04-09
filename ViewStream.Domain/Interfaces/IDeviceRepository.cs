using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Device entity
    /// </summary>
    public interface IDeviceRepository : IGenericRepository<Device>
    {
        // TODO: Add custom methods specific to Device here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Device>> GetActiveAsync();
    }
}
