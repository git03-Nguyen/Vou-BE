using AuthServer.Data.Contexts;
using AuthServer.Data.Entities;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implementation;

public class ClientScopeRepository :  GenericRepository<AuthDbContext, ClientScope>, IClientScopeRepository
{
    public ClientScopeRepository(AuthDbContext context) : base(context)
    {
    }
}