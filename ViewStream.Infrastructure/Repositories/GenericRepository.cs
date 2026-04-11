using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.Persistence;

namespace ViewStream.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ViewStreamDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ViewStreamDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            IQueryable<T> query = _dbSet;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.CountAsync(cancellationToken);
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
