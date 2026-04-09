using System.Linq.Expressions;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>Get entity by id</summary>
        Task<T?> GetByIdAsync(int id);
        
        /// <summary>Get all entities</summary>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>Find entities by predicate</summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>Get single entity or default</summary>
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>Add new entity</summary>
        Task AddAsync(T entity);
        
        /// <summary>Add multiple entities</summary>
        Task AddRangeAsync(IEnumerable<T> entities);
        
        /// <summary>Update existing entity</summary>
        void Update(T entity);
        
        /// <summary>Remove entity</summary>
        void Remove(T entity);
        
        /// <summary>Remove multiple entities</summary>
        void RemoveRange(IEnumerable<T> entities);
        
        /// <summary>Count total entities</summary>
        Task<int> CountAsync();
        
        /// <summary>Check if any entity matches predicate</summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
