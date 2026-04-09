using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PaymentMethod entity
    /// </summary>
    public interface IPaymentMethodRepository : IGenericRepository<PaymentMethod>
    {
        // TODO: Add custom methods specific to PaymentMethod here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<PaymentMethod>> GetActiveAsync();
    }
}
