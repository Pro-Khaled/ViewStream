using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Invoice entity
    /// </summary>
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        // TODO: Add custom methods specific to Invoice here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<Invoice>> GetActiveAsync();
    }
}
