using System.Linq.Expressions;

namespace Shared.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> AddRangeAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default);
        bool Update(TEntity entity);
        bool UpdateRange(IEnumerable<TEntity> entities);
        bool Remove(TEntity entity);
        bool RemoveRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);
    }
}
