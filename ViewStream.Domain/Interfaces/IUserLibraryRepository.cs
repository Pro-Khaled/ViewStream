using ViewStream.Domain.Entities;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserLibrary entity
    /// </summary>
    public interface IUserLibraryRepository : IGenericRepository<UserLibrary>
    {
        // TODO: Add custom methods specific to UserLibrary here
        // Example:
        // Task<> GetByNameAsync(string name);
        // Task<IEnumerable<UserLibrary>> GetActiveAsync();
    }
}
