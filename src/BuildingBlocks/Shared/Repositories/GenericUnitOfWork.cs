using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shared.Repositories;

public abstract class GenericUnitOfWork<TDbContext> : IGenericUnitOfWork, IAsyncDisposable where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    protected IDbContextTransaction? Transaction;

    public GenericUnitOfWork(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default)
    {
        await DisposeTransactionAsync();
        Transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
        return Transaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null)
        {
            return;
        }

        try
        {
            await Transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null)
        {
            return;
        }

        try
        {
            await Transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync();
        await DbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected async Task DisposeTransactionAsync()
    {
        if (Transaction is null)
        {
            return;
        }

        await Transaction.DisposeAsync();
        Transaction = null;
    }
}
