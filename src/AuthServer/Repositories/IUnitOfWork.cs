using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IUserRepository User { get; }
    IClientRepository Client { get; }
    IClientGrantTypeRepository ClientGrantType { get; }
    IClientSecretRepository ClientSecret { get; }
    IClientScopeRepository ClientScope { get; }
}