using System.Linq.Expressions;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Finds entities matching the predicate with optional eager loading.
        /// </summary>
        /// <param name="predicate">Filter expression.</param>
        /// <param name="include">Optional function to include navigation properties.</param>
        /// <param name="asNoTracking">If true, entities are not tracked by the change tracker (improves read performance).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of matching entities.</returns>
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an entity by its primary key.
        /// </summary>
        Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple entities.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Checks if any entity matches the predicate.
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts entities matching the predicate.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    }
}
