using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PromoCode entity
    /// </summary>
    public interface IPromoCodeRepository : IGenericRepository<PromoCode>
    {
        // TODO: Add custom methods specific to PromoCode here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<PromoCode>> GetActiveAsync();
    }
}
