using AuthServer.Data.Contexts;
using AuthServer.Data.Entities;
using AuthServer.Repositories.Interfaces;
using Shared.Repositories;

namespace AuthServer.Repositories.Implementation;

public class ClientRepository :  GenericRepository<AuthDbContext, Client>, IClientRepository
{
    public ClientRepository(AuthDbContext context) : base(context)
    {
    }
}