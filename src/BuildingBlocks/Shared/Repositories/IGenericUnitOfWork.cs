using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shared.Repositories;

public interface IGenericUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> OpenTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}