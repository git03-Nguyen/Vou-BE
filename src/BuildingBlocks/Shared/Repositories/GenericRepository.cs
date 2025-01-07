using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared.Repositories;

public abstract class GenericRepository<TContext, TEntity> 
    : IGenericRepository<TEntity> 
    where TContext : DbContext 
    where TEntity : class
{
    protected readonly TContext Context;
    private readonly DbSet<TEntity> _dbSet;
    protected GenericRepository(TContext context)
    {
        Context = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbSet;
    }

    public async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return true;
    }

    public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entity, cancellationToken);
        return true;
    }

    public bool Update(TEntity entity)
    {
        _dbSet.Update(entity);
        return true;
    }

    public bool UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        return true;
    }

    public bool Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
        return true;
    }

    public bool RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return true;
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
    {
        return _dbSet.Where(expression);
    }
}