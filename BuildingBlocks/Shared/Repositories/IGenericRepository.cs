using System.Linq.Expressions;

namespace Shared.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> AddRangeAsync(IEnumerable<T> entity, CancellationToken cancellationToken = default);
        bool Update(T entity);
        bool UpdateRange(IEnumerable<T> entities);
        bool Remove(T entity);
        bool RemoveRange(IEnumerable<T> entities);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
    }
}
