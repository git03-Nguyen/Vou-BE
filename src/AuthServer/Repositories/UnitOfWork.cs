using AuthServer.Data.Contexts;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories;

public class UnitOfWork : GenericUnitOfWork<AuthDbContext>, IUnitOfWork
{
    public UnitOfWork(AuthDbContext dbContext) : base(dbContext)
    {
    }
    public IUserRepository User { get; }
    public IClientRepository Client { get; }
    public IClientGrantTypeRepository ClientGrantType { get; }
    public IClientSecretRepository ClientSecret { get; }
    public IClientScopeRepository ClientScope { get; }
}